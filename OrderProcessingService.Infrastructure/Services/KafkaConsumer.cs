using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OrderProcessingService.Application.Abstarctions;
using OrderProcessingService.Application.Models.Kafka;
using OrderProcessingService.Domain.Abstractions;
using OrderProcessingService.Domain.Entities;
using OrderProcessingService.Domain.Enums;
using OrderProcessingService.Infrastructure.Kafka;

namespace OrderProcessingService.Infrastructure.Services;

public class KafkaConsumer<TMessage> : IKafkaConsumer<TMessage>
{
    private readonly IConsumer<string, TMessage> _consumer;
    private readonly string _topic;
    private readonly IOrderProcessingRepository _processingRepository;

    public KafkaConsumer(IOptions<KafkaSettings> kafkaSettings, IOrderProcessingRepository processingRepository)
    {
        _processingRepository = processingRepository;

        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = kafkaSettings.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, TMessage>(config)
            .SetValueDeserializer(new KafkaJsonDeserializer<TMessage>())
            .Build();

        _topic = kafkaSettings.Value.Topic;
    }

    public void Consume(CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                Console.WriteLine($"Получено сообщение: {consumeResult.Message.Value}");

                ProcessMessage(consumeResult.Message.Value, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            //Как обработать ?
        }
        finally
        {
            _consumer.Close();
        }
    }

    private void ProcessMessage(TMessage message, CancellationToken ct)
    {
        if (message is CreateOrderKafkaModel orderMessage)
        {
            var processingOrder = new ProcessingOrder()
            {
                Id = Guid.NewGuid(),
                OrderId = orderMessage.Id,
                Items = orderMessage.Items.Select(f => new Item()
                {
                    ProductId = f.ProductId,
                    Status = OrderStatus.Pending,
                    Quantity = f.Quantity,
                }).ToList()
            };

            _processingRepository.CreateAsync(processingOrder, ct);
        }
    }

    public void Dispose()
    {
        _consumer?.Dispose();
    }
}