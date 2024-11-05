using Confluent.Kafka;

namespace Messaging.Kafka.Kafka;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, Message<string, string> message);
}