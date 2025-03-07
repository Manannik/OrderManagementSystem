using Messaging.Kafka.Models;
using Microsoft.OpenApi.Extensions;
using Notification.Domain.Abstractions;
using Notification.Domain.Entities;
using Notification.Domain.Entities.User;
using Notification.Infrastructure.Telegram.Models;
using Telegram.Bot;

namespace Messaging.Kafka;

public class OrderNotificationMessageHandler(
    IUserStateRepository userStateRepository,
    IUserDataRepository userDataRepository,
    ITelegramBotClient botClient) : IMessageHandler<NotificationKafkaModel>
{
    public async Task HandleAsync(
        NotificationKafkaModel message,
        CancellationToken cancellationToken)
    {
        if (message is not NotificationKafkaModel notification)
        {
            return;
        }

        var existingOrder = await userDataRepository
            .TryGetByOrderIdAsync(notification.OrderId, cancellationToken);

        if (existingOrder == null)
        {
            var userData = new UserData()
            {
                OrderId = notification.OrderId,
                Stage = (Stage)notification.Stage
            };
            await userDataRepository.CreateAsync(userData, cancellationToken);

            var userState = await userStateRepository.TryGetByOrderIdAsync(
                userData.OrderId,
                cancellationToken
            );

            if (userState != null)
            {
                await SendStatusMessageAsync(userState, userData.OrderId, cancellationToken);
            }
        }
        else
        {
            existingOrder.Stage = (Stage)message.Stage;
            await userDataRepository.UpdateAsync(existingOrder, cancellationToken);

            var userState = await userStateRepository.TryGetByOrderIdAsync(
                existingOrder.OrderId,
                cancellationToken
            );

            if (userState != null)
            {
                await SendStatusMessageAsync(userState, existingOrder.OrderId, cancellationToken);
            }
        }
    }

    private async Task SendStatusMessageAsync(
        UserState userState,
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var userStateModel = ToUserStateModel(userState);
        var statusDisplayName = userStateModel.OrderStatus.GetDisplayName();
        var message = $"Заказ {orderId} находится в статусе {statusDisplayName}";
        await botClient.SendMessage(
            chatId: userState.ChatId,
            text: message,
            cancellationToken: cancellationToken);
    }

    private UserStateModel ToUserStateModel(UserState userState)
    {
        var userStateModel = new UserStateModel
        {
            TelegramUserId = userState.TelegramUserId,
            OrderStatus = (OrderStatusModel)userState.UserData.Stage
        };
        return userStateModel;
    }
}