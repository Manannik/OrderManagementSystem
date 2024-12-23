using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka;

public class KafkaConsumer<TMessage> : BackgroundService
{
    private readonly IMessageHandler<TMessage> _messageHandler;
    private readonly string _topic;
    private readonly IConsumer<string, TMessage> _consumer;

    public KafkaConsumer(IOptions<KafkaSetting> kafkaSettings, IMessageHandler<TMessage> messageHandler)
    {
        _messageHandler = messageHandler;
        var config = new ConsumerConfig()
        {
            AutoOffsetReset = AutoOffsetReset.Earliest,
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = kafkaSettings.Value.GroupId
        };

        _topic = kafkaSettings.Value.Topic;

        _consumer = new ConsumerBuilder<string, TMessage>(config).SetValueDeserializer(new KafkaDeserializer<TMessage>()).Build();
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
                await _messageHandler.HandleAsync(result.Message.Value, stoppingToken);
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