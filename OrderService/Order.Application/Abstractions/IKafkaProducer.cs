using Confluent.Kafka;

namespace Order.Application.Abstractions
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, Message<string, string> message);
    }
}