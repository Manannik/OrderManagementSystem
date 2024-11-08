using Confluent.Kafka;

namespace Order.Infrastructure.Kafka;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, Message<string, string> message);
}