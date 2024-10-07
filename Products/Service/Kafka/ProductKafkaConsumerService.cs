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
        private readonly IReadProductRepository _privateProductReadRepository;
        private readonly KafkaConfig _kafkaConfig;
        private readonly ILogger<ProductKafkaConsumerService> _logger;
        private readonly List<string> _topics;
        private readonly IConsumer<string, string> _consumer;

        public ProductKafkaConsumerService(IOptions<KafkaConfig> kafkaConfig, ILogger<ProductKafkaConsumerService> logger, IReadProductRepository privateProductReadRepository)
        {
            _privateProductReadRepository = privateProductReadRepository;
            _kafkaConfig = kafkaConfig.Value;
            _logger = logger;
            // Lista de tópicos que o consumidor vai ler
            _topics = new List<string>
            {
                KafkaTopics.InsertProductTopic,
                KafkaTopics.DeleteProductTopic,
                KafkaTopics.UpdateProductTopic
            };
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                GroupId = _kafkaConfig.ConsumerGroupId, // Adicionar um GroupId para o consumidor
                AutoOffsetReset = AutoOffsetReset.Earliest, // Garantir que comece do início se não houver offsets salvos
                EnableAutoCommit = true // Commit manual após processamento
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
                            _consumer.Commit();
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Erro ao consumir mensagem: {e.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(50), stoppingToken);
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

        public static Acks ParseAcks(string acksValue)
        {
            // Converter string para enum Acks
            return acksValue.ToLower() switch
            {
                "all" => Acks.All,
                "none" => Acks.None,
                "leader" => Acks.Leader,
                _ => throw new ArgumentException($"Valor inválido para Acks: {acksValue}")
            };
        }
        private async Task ProcessMessageAsync(string topic, string key, string value, CancellationToken stoppingToken)
        {

            // Processamento personalizado para cada tópico
            switch (topic)
            {
                case KafkaTopics.InsertProductTopic:
                    _logger.LogInformation($"Processando mensagem de compra de investimento. Key: {key}, Value: {value}");
                    await _privateProductReadRepository.InsertAsync(JsonConvert.DeserializeObject<ProductDB>(value), stoppingToken);
                    break;
                case KafkaTopics.InvestmentPurchasedTopic:
                    _logger.LogInformation($"Processando mensagem de compra de investimento. Key: {key}, Value: {value}");
                    // Adicionar lógica de processamento para o evento de compra de investimento
                    break;

                case KafkaTopics.InvestmentSoldTopic:
                    _logger.LogInformation($"Processando mensagem de venda de investimento. Key: {key}, Value: {value}");
                    // Adicionar lógica de processamento para o evento de venda de investimento
                    break;

                case KafkaTopics.UpdateProductTopic:
                    _logger.LogInformation($"Processando mensagem de atualização de produto. Key: {key}, Value: {value}");
                    await _privateProductReadRepository.UpdateAsync(JsonConvert.DeserializeObject<ProductDB>(value), stoppingToken);
                    break;

                case KafkaTopics.DeleteProductTopic:
                    _logger.LogInformation($"Processando mensagem de atualização de produto. Key: {key}, Value: {value}");
                    await _privateProductReadRepository.DeleteAsync(JsonConvert.DeserializeObject<ProductDB>(value).Id, stoppingToken);
                    break;

                case KafkaTopics.ProductExpiryNotificationTopic:
                    _logger.LogInformation($"Processando mensagem de notificação de expiração de produto. Key: {key}, Value: {value}");
                    // Adicionar lógica de processamento para o evento de notificação de expiração de produto
                    break;

                default:
                    _logger.LogWarning($"Tópico desconhecido: {topic}");
                    break;
            }
        }
    }

}
