using OrderProcessingService.Application.Abstarctions;

namespace OrderProcessingService.Web.Services;

public class KafkaConsumerBackgroundService<TMessage> : BackgroundService
{
    private readonly IKafkaConsumer<TMessage> _kafkaConsumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<KafkaConsumerBackgroundService<TMessage>> _logger;

    public KafkaConsumerBackgroundService(
        IKafkaConsumer<TMessage> kafkaConsumer,
        ILogger<KafkaConsumerBackgroundService<TMessage>> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _kafkaConsumer = kafkaConsumer;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer Background Service Стартанул");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var kafkaConsumer = scope.ServiceProvider.GetRequiredService<IKafkaConsumer<TMessage>>();
                    await kafkaConsumer.ConsumeAsync(stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ОШИБКА");
        }

        _logger.LogInformation("Kafka Consumer Background Service ОСТАНОВИЛСЯ");
    }

    public override void Dispose()
    {
        _kafkaConsumer.Dispose();
        base.Dispose();
    }
}