using Confluent.Kafka;

namespace Order.Persistence.Kafka;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, Message<string, string> message);
}