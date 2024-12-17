namespace OrderProcessingService.Application.Abstarctions;

public interface IKafkaConsumer<T>:IDisposable
{
    public Task ConsumeAsync(CancellationToken cancellationToken);
}