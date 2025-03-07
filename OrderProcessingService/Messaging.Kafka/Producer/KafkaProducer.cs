using Confluent.Kafka;
using Messaging.Kafka.Models;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Producer
{
    public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
    {
        private readonly IProducer<string, TMessage> producer;
        private readonly string topic;
        public KafkaProducer(IOptions<NotificationKafkaSettings> kafkaSettings)
        {
            var config = new ProducerConfig()
            {
                BootstrapServers = kafkaSettings.Value.BootstrapServers
            };

            producer = new ProducerBuilder<string, TMessage>(config)
                .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
                .Build();

            topic = kafkaSettings.Value.Topic;
        }

        public async Task ProduceAsync(TMessage message, CancellationToken cancellationToken)
        {
            var orderId = (message as NotificationKafkaModel)!.OrderId.ToString();
            
            await producer.ProduceAsync(topic, new Message<string, TMessage>()
            {
                Key = orderId,
                Value = message
            }, cancellationToken);
        }

        public void Dispose()
        {
            producer?.Dispose();
        }
    }
}