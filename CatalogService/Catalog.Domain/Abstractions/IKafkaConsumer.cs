namespace Domain.Abstractions;

public interface IKafkaConsumer<in TMessage> : IDisposable
{
    Task ConsumeAsync(TMessage message, CancellationToken cancellationToken);
}
