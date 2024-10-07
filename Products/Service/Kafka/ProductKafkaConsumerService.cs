using Confluent.Kafka;
using Infrastructure.Repository.Entities;
using Investments.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Products.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KafkaConfig = Infrastructure.Kafka.KafkaConfig;

namespace Products.Service.Kafka
{
    internal class ProductKafkaConsumerService : BackgroundService
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly ILogger<ProductKafkaConsumerService> _logger;
        private readonly List<string> _topics;
        private readonly IConsumer<string, string> _consumer;

        public ProductKafkaConsumerService(IOptions<KafkaConfig> kafkaConfig, ILogger<ProductKafkaConsumerService> logger)
        {
            _kafkaConfig = kafkaConfig.Value;
            _logger = logger;

            // Lista de tópicos que o consumidor vai ler
            _topics = new List<string>
            {
                KafkaTopics.InsertProductTopic,
                KafkaTopics.DeleteProductTopic,
                KafkaTopics.InvestmentPurchasedTopic,
                KafkaTopics.InvestmentSoldTopic,
                KafkaTopics.UpdateProductTopic,
                KafkaTopics.ProductExpiryNotificationTopic
            };

            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                GroupId = _kafkaConfig.ConsumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_topics);
            _logger.LogInformation($"Inscrito nos tópicos: {string.Join(", ", _topics)}");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(stoppingToken);
                        if (consumeResult != null)
                        {
                            _logger.LogInformation($"Mensagem recebida do tópico {consumeResult.Topic}. Key: {consumeResult.Message.Key}, Value: {consumeResult.Message.Value}");
                            await ProcessMessageAsync(consumeResult.Topic, consumeResult.Message.Key, consumeResult.Value, stoppingToken);
                            _consumer.Commit(consumeResult);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Erro ao consumir mensagem: {e.Error.Reason}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro inesperado: {ex.Message}");
                    }

                    // O tempo de espera pode ser ajustado conforme a necessidade
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumo de Kafka cancelado.");
            }
            finally
            {
                _consumer.Close();
            }
        }

        private async Task ProcessMessageAsync(string topic, string key, string value, CancellationToken stoppingToken)
        {
            // Processamento personalizado para cada tópico
            switch (topic)
            {
                case KafkaTopics.InsertProductTopic:
                    _logger.LogInformation($"Processando mensagem de inserção de produto. Key: {key}, Value: {value}");
                    // await _repository.InsertAsync(JsonConvert.DeserializeObject<ProductDB>(value), stoppingToken);
                    break;

                case KafkaTopics.UpdateProductTopic:
                    _logger.LogInformation($"Processando mensagem de atualização de produto. Key: {key}, Value: {value}");
                    // await _repository.UpdateAsync(JsonConvert.DeserializeObject<ProductDB>(value), stoppingToken);
                    break;

                case KafkaTopics.DeleteProductTopic:
                    _logger.LogInformation($"Processando mensagem de exclusão de produto. Key: {key}, Value: {value}");
                    // await _repository.DeleteAsync(JsonConvert.DeserializeObject<ProductDB>(value).Id, stoppingToken);
                    break;

                default:
                    _logger.LogWarning($"Tópico desconhecido: {topic}");
                    break;
            }
        }
    }
}