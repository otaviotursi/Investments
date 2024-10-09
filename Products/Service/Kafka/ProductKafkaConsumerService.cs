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
        private readonly IServiceProvider _serviceProvider; // Injeção de IServiceProvider
        private readonly ILogger<ProductKafkaConsumerService> _logger;
        private readonly List<string> _topics;
        private readonly IConsumer<string, string> _consumer;

        public ProductKafkaConsumerService(IServiceProvider serviceProvider, IOptions<KafkaConfig> kafkaConfig, ILogger<ProductKafkaConsumerService> logger)
        {
            _kafkaConfig = kafkaConfig.Value;
            _serviceProvider = serviceProvider; // Guardar o IServiceProvider
            _logger = logger;

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
                EnableAutoCommit = false
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
                            _logger.LogInformation($"Mensagem recebida do tópico {consumeResult.Topic}: {consumeResult.Message.Value}");

                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var repository = scope.ServiceProvider.GetRequiredService<IReadProductRepository>();
                                await ProcessMessageAsync(consumeResult.Topic, consumeResult.Message.Key, consumeResult.Message.Value, repository, stoppingToken);
                            }

                            _consumer.Commit();
                        }

                        await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError($"Erro ao consumir mensagem: {ex.Error.Reason}");
                        // Continue mesmo com erro
                    }
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

        private async Task ProcessMessageAsync(string topic, string key, string value, IReadProductRepository repository, CancellationToken stoppingToken)
        {
            // Processamento da mensagem usando o repository scoped
            switch (topic)
            {
                case KafkaTopics.InsertProductTopic:
                    _logger.LogInformation($"Processando mensagem de inserção. Key: {key}, Value: {value}");
                    var productInsert = JsonConvert.DeserializeObject<ProductDB>(value);
                    await repository.InsertAsync(productInsert, stoppingToken);
                    break;

                case KafkaTopics.UpdateProductTopic:
                    _logger.LogInformation($"Processando mensagem de atualização. Key: {key}, Value: {value}");
                    var productUpdate = JsonConvert.DeserializeObject<ProductDB>(value);
                    await repository.UpdateAsync(productUpdate, stoppingToken);
                    break;

                case KafkaTopics.DeleteProductTopic:
                    _logger.LogInformation($"Processando mensagem de exclusão. Key: {key}, Value: {value}");
                    var productDelete = JsonConvert.DeserializeObject<ProductDB>(value);
                    await repository.DeleteAsync(productDelete.Id, stoppingToken);
                    break;

                default:
                    _logger.LogWarning($"Tópico desconhecido: {topic}");
                    break;
            }
        }
    }

}