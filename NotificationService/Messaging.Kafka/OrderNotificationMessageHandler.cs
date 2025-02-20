using Messaging.Kafka.Models;

namespace Messaging.Kafka;

public class OrderNotificationMessageHandler : IMessageHandler<NotificationKafkaModel>
{
    public async Task HandleAsync(NotificationKafkaModel message, CancellationToken cancellationToken)
    {
        // logger.LogInformation($"Заказ создан. Начинаем обработку {message.Id}");
        if (message is NotificationKafkaModel notification)
        {
            // var processingOrder = new ProcessingOrder()
            // {
            //     Id = Guid.NewGuid(),
            //     OrderId = orderMessage.Id,
            //     Items = orderMessage.ProductItemModels.Select(f => new ProcessingOrderItem()
            //     {
            //         ProductId = f.ProductId,
            //         ProcessingOrderItemStatus = ProcessingOrderItemStatus.Pending,
            //         Quantity = f.Quantity,
            //     }).ToList(),
            // };
            // await orderProcessingRepository.CreateAsync(processingOrder, cancellationToken);
        }
    }
}