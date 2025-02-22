using Messaging.Kafka.Models;
using Notification.Application.Services;

namespace Messaging.Kafka;

public class OrderNotificationMessageHandler(TelegramMessageService telegramMessageService) : IMessageHandler<NotificationKafkaModel>
{
    public async Task HandleAsync(NotificationKafkaModel message, CancellationToken cancellationToken)
    {
        // logger.LogInformation($"Заказ создан. Начинаем обработку {message.Id}");
        if (message is NotificationKafkaModel notification)
        {
            long chatId = 7115353156;
            string messageText = "Hello from TG BOT";

            await telegramMessageService.SendTelegramMessageAsync(chatId, messageText);
        }
    }
}