using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Abstractions;
using Notification.Domain.Entities.User;
using Notification.Infrastructure.Telegram.Models;
using Notification.Infrastructure.Telegram.Pages;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;

namespace Notification.Infrastructure.Telegram;

public class UpdateHandler(IServiceScopeFactory scopeFactory, PagesFactory pagesFactory) : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message && update.Type != UpdateType.CallbackQuery)
        {
            return;
        }

        var telegramUserId = update.Type == UpdateType.Message
            ? update.Message.From.Id
            : update.CallbackQuery.From.Id;

        Console.WriteLine($"Update ID: {update.Id}, Telegram User ID: {telegramUserId}");
        using var scope = scopeFactory.CreateScope();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

        var userState = await notificationRepository.TryGetUserStateByTelegramIdAsync(telegramUserId, cancellationToken);

        if (userState == null)
        {
            userState = new UserState()
            {
                TelegramUserId = telegramUserId,
                Pages = [nameof(NotStartedPage)]
            };

            await notificationRepository.CreateUserStateAsync(userState, cancellationToken);
        }
        
        var userStateModel = ToUserStateModel(userState);
            
        Console.WriteLine($"Update ID: {update.Id}, Current User State: {userState}");

        var result = await userStateModel.CurrentPage.Handle(update, userStateModel);
        Console.WriteLine(
            $"Update ID: {update.Id}, Send Text: {result.Text}, Updated User State: {result.UpdatedUserStateModel}");

        var lastMessage = await SendMessage(botClient, update, telegramUserId, result);
        result.UpdatedUserStateModel.LastMessageId = lastMessage.MessageId;

        var updatedUserState = ToUserState(result, telegramUserId, lastMessage);

        await notificationRepository.UpdateUserStateAsync(updatedUserState, cancellationToken);
    }

    private static UserState ToUserState(PageResult result, long telegramUserId, Message lastMessage)
    {
        var pages = result.UpdatedUserStateModel.Pages.Select(page => page.GetType().Name)
            .ToList();

        var updatedUserState = new UserState
        {
            TelegramUserId = telegramUserId,
            Pages = pages,
            LastUserMessage = new UserMessage(lastMessage.MessageId)
        };
        return updatedUserState;
    }

    private UserStateModel ToUserStateModel(UserState userState)
    {
        var pages = userState.Pages.Select(f => pagesFactory.GetPage(f)).Reverse();
        var userStateModel = new UserStateModel { TelegramUserId = userState.TelegramUserId, Pages = new Stack<IPage>(pages) };
        return userStateModel;
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
    }

    private static async Task<Message> SendMessage(
        ITelegramBotClient client,
        Update update,
        long telegramUserId,
        PageResult result)
    {
        if (update.CallbackQuery != null && result.UpdatedUserStateModel.LastMessageId.HasValue)
        {
            return await client.EditMessageText(
                chatId: telegramUserId,
                messageId: result.UpdatedUserStateModel.LastMessageId.Value,
                text: result.Text,
                parseMode: ParseMode.Html,
                replyMarkup: result.ReplyMarkup as InlineKeyboardMarkup,
                cancellationToken: default);
        }

        if (result.UpdatedUserStateModel.LastMessageId.HasValue)
        {
            await client.DeleteMessage(
                chatId: telegramUserId,
                messageId: result.UpdatedUserStateModel.LastMessageId.Value);
        }

        return await client.SendMessage(
            chatId: telegramUserId,
            text: result.Text,
            replyMarkup: result.ReplyMarkup,
            cancellationToken: default);
    }
}