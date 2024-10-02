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
    public class DeleteProductEventHandler : INotificationHandler<DeleteProductEvent>
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        public DeleteProductEventHandler(IKafkaProducerService kafkaProducerService)
        {
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(DeleteProductEvent productEvent, CancellationToken cancellationToken)
        {
            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.DeleteProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent));
        }
    }
}
