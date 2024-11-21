using Confluent.Kafka;
using Domain.Abstractions;

namespace WebApplication.Kafka;

public class KafkaConsumer<TMessage> : IKafkaConsumer<TMessage>
{
    private readonly IConsumer<string, TMessage> consumer;
    public Task ConsumeAsync(TMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        consumer?.Dispose();
    }
}