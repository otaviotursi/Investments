using Infrastructure.Services;
using Investments.Infrastructure.Kafka;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Event.Handler
{
    public class CreateProductEventHandler : INotificationHandler<CreateProductEvent>
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        public CreateProductEventHandler(IKafkaProducerService kafkaProducerService)
        {
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(CreateProductEvent productEvent, CancellationToken cancellationToken)
        {
            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.InsertProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent));
        }
    }
}
