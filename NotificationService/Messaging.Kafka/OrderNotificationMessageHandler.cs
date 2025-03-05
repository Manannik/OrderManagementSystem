using Messaging.Kafka.Models;
using Notification.Application.Services;
using Notification.Domain.Abstractions;
using Notification.Domain.Entities;
using Notification.Domain.Entities.User;

namespace Messaging.Kafka;

public class OrderNotificationMessageHandler(INotificationRepository notificationRepository) : IMessageHandler<NotificationKafkaModel>
{
    public async Task HandleAsync(NotificationKafkaModel message, CancellationToken cancellationToken)
    {
        // logger.LogInformation($"Заказ создан. Начинаем обработку {message.Id}");
        if (message is NotificationKafkaModel notification)
        {
            var existingOrder = await notificationRepository
                .TryGetUserDataByOrderIdAsync(message.OrderId,cancellationToken);

            if (existingOrder == null)
            {
                var userData = new UserData()
                {
                    OrderId = notification.OrderId,
                    Stage = (Stage)notification.Stage
                };
                await notificationRepository.CreateUserDataAsync(userData, cancellationToken);
            }

            else
            {
                existingOrder.Stage = (Stage)message.Stage;
                await notificationRepository.UpdateUserDataAsync(existingOrder, cancellationToken);
            }
        }
    }
}