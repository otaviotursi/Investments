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
    public class UpdateProductEventHandler : INotificationHandler<UpdateProductEvent>
    {
        private readonly IKafkaProducerService _kafkaProducerService;
        public UpdateProductEventHandler(IKafkaProducerService kafkaProducerService)
        {
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task Handle(UpdateProductEvent productEvent, CancellationToken cancellationToken)
        {
            await _kafkaProducerService.PublishMessageAsync(KafkaTopics.UpdateProductTopic, productEvent.Id.ToString(), JsonConvert.SerializeObject(productEvent));
        }
    }
}
