namespace OrderProcessingService.Application.Abstarctions;

public interface IKafkaConsumer<T>:IDisposable
{
    public void Consume(CancellationToken cancellationToken);
}