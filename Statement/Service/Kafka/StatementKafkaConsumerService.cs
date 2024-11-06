﻿using Confluent.Kafka;
using Infrastructure.Repository.Entities;
using Investments.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KafkaConfig = Infrastructure.Kafka.KafkaConfig;
using IEmailNotificationService = Infrastructure.Email.Interface.IEmailNotificationService;
using Infrastructure.Email;
using Statement.Repository.Interface;


namespace Statement.Service.Kafka
{
    internal class StatementKafkaConsumerService : BackgroundService
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly EmailConfig _emailConfig;
        private readonly int _daysToExpiration = 7;
        private readonly IServiceProvider _serviceProvider; // Injeção de IServiceProvider
        private readonly ILogger<StatementKafkaConsumerService> _logger;
        private readonly List<string> _topics;
        private readonly IConsumer<string, string> _consumer;


        public StatementKafkaConsumerService(IServiceProvider serviceProvider, IOptions<KafkaConfig> kafkaConfig, ILogger<StatementKafkaConsumerService> logger, IEmailNotificationService emailNotificationService, IOptions<EmailConfig> emailConfig)
        {
            _emailConfig = emailConfig.Value;
            _kafkaConfig = kafkaConfig.Value;
            _serviceProvider = serviceProvider; // Guardar o IServiceProvider
            _logger = logger;
            // Lista de tópicos que o consumidor vai ler
            _topics = new List<string>
            {
                KafkaTopics.InsertCustomerPorftolioStatement
            };
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                GroupId = _kafkaConfig.ConsumerGroupId, // Adicionar um GroupId para o consumidor
                AutoOffsetReset = AutoOffsetReset.Earliest, // Garantir que comece do início se não houver offsets salvos
                EnableAutoCommit = false // Commit manual após processamento
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
                                var repository = scope.ServiceProvider.GetRequiredService<IPortfolioStatementRepository>();
                                await ProcessMessageAsync(consumeResult.Topic, consumeResult.Message.Key, consumeResult.Message.Value, repository, stoppingToken);
                            }

                            _consumer.Commit();
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Erro ao consumir mensagem: {e.Message}");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
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



        private async Task ProcessMessageAsync(string topic, string key, string value, IPortfolioStatementRepository repository, CancellationToken stoppingToken)
        {
            // Processamento da mensagem usando o repository scoped
            switch (topic)
            {
                case KafkaTopics.InsertCustomerPorftolioStatement:
                    _logger.LogInformation($"Processando mensagem de inserção. Key: {key}, Value: {value}");
                    var insertProduct = JsonConvert.DeserializeObject<PortfolioStatementDB>(value);
                    await repository.InsertAsync(insertProduct, stoppingToken);
                    break;

                default:
                    _logger.LogWarning($"Tópico desconhecido: {topic}");
                    break;
            }
        }

    }

}