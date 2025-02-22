using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Common.Pages;
using Telegram.Common.Storage;
using Telegram.Common.User;

namespace Telegram.Common;

public class UpdateHandler(UserStateStorage storage,IServiceProvider services) : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != Bot.Types.Enums.UpdateType.Message
            && update.Type != Bot.Types.Enums.UpdateType.CallbackQuery)
        {
            return;
        }

        long telegramUserId;
        if (update.Type == Bot.Types.Enums.UpdateType.Message)
        {
            telegramUserId = update.Message.From.Id;
        }
        else
        {
            telegramUserId = update.CallbackQuery.From.Id;
        }
        Console.WriteLine($"update_id = ${update.Id}, telegramUserId = ${telegramUserId}");

        var userState = await storage.TryGetAsync(telegramUserId);

        if (userState == null)
        {
            userState = new UserState(new Stack<IPage>([services.GetRequiredService<NotStartedPage>()]), new UserData());
        }
        Console.WriteLine($"update_id = ${update.Id}, CURRENT_UserState = ${userState}");

        var result = userState!.CurrentPage.Handle(update, userState);
        Console.WriteLine($"update_id = ${update.Id}, send_text = ${result.Text}, UPDATED_UserState={result.UpdatedUserState}");

        var lastMessage = await SendMessage(botClient, update, telegramUserId, result);

        result.UpdatedUserState.UserData.LastMessage = new User.Message(lastMessage.MessageId);

        await storage.AddOrUpdateAsync(telegramUserId, result.UpdatedUserState);
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
    }

    private static async Task<Bot.Types.Message> SendMessage(
        ITelegramBotClient client,
        Update update,
        long telegramUserId,
        PageResult result)
    {
        if (update.CallbackQuery != null && result.UpdatedUserState.UserData.LastMessage != null)
        {
            return await client.EditMessageText(
                chatId: telegramUserId,
                messageId: result.UpdatedUserState.UserData.LastMessage.Id,
                text: result.Text,
                parseMode: Bot.Types.Enums.ParseMode.Html,
                replyMarkup: (InlineKeyboardMarkup)result.ReplyMarkup);
        }

        if (result?.UpdatedUserState?.UserData?.LastMessage != null)
        {
            await client.DeleteMessage(chatId: telegramUserId,
                messageId: result.UpdatedUserState.UserData.LastMessage.Id);
        }

        return await client.SendMessage(
            chatId: telegramUserId,
            text: result.Text,
            replyMarkup: result.ReplyMarkup);
    }
}