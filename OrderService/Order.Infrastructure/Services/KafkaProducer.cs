using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Order.Application.Abstractions;
using Order.Infrastructure.Kafka;

namespace Order.Infrastructure.Services
{
    public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
    {
        private readonly IProducer<string, TMessage> producer;
        private readonly string topic;

        public KafkaProducer(IOptions<KafkaSettings> kafkaSettings)
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
            await producer.ProduceAsync(topic, new Message<string, TMessage>()
            {
                Key="uniq1",
                Value = message
            },cancellationToken);
        }

        public void Dispose()
        {
            producer?.Dispose();
        }
    }
}