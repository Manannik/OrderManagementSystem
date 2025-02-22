using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Consumer;

public class KafkaConsumer<TMessage> : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _topic;
    private readonly IConsumer<string, TMessage> _consumer;

    public KafkaConsumer(IOptions<KafkaSettings> kafkaSettings, IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        var config = new ConsumerConfig()
        {
            AutoOffsetReset = AutoOffsetReset.Earliest,
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = kafkaSettings.Value.GroupId,
            EnableAutoCommit = false,
        };

        _topic = kafkaSettings.Value.Topic;

        _consumer = new ConsumerBuilder<string, TMessage>(config)
            .SetValueDeserializer(new KafkaDeserializer<TMessage>()).Build();
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
    }

    private async Task? ConsumeAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = _consumer.Consume(stoppingToken);
                using var scope = _serviceScopeFactory.CreateScope();
                var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();
                await messageHandler.HandleAsync(result.Message.Value, stoppingToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Close();
        return base.StopAsync(cancellationToken);
    }
}