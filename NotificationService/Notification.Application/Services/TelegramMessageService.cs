using Telegram.Bot;

namespace Notification.Application.Services;

public class TelegramMessageService
{
    private readonly ITelegramBotClient _botClient;

    public TelegramMessageService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task SendTelegramMessageAsync(long chatId, string text)
    {
        try
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
            );
            Console.WriteLine("Сообщение успешно отправлено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
        }
    }
}