using Messaging.Kafka.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderProcessingService.Domain.Abstractions;
using OrderProcessingService.Domain.Entities;
using OrderProcessingService.Domain.Enums;

namespace Messaging.Kafka;

public class OrderCreatedMessageHandler(
    ILogger<OrderCreatedMessageHandler> logger,
    IOrderProcessingRepository orderProcessingRepository) : IMessageHandler<OrderCreated>
{
    public async Task HandleAsync(OrderCreated message, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Заказ создан. Начинаем обработку {message.Id}");
        if (message is OrderCreated orderMessage)
        {
            var processingOrder = new ProcessingOrder()
            {
                Id = Guid.NewGuid(),
                OrderId = orderMessage.Id,
                Items = orderMessage.Items.Select(f => new ProcessingOrderItem()
                {
                    ProductId = f.ProductId,
                    ProcessingOrderItemStatus = ProcessingOrderItemStatus.Pending,
                    Quantity = f.Quantity,
                }).ToList(),
            };
            await orderProcessingRepository.CreateAsync(processingOrder, cancellationToken);
        }
    }
}