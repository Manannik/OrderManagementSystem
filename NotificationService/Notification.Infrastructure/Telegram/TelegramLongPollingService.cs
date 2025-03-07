using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Notification.Infrastructure.Telegram;

public class TelegramLongPollingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TelegramLongPollingService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            var handler = scope.ServiceProvider.GetRequiredService<IUpdateHandler>();

            try
            {
                var user = await client.GetMe(stoppingToken);
                Console.WriteLine($"Начали слушать апдейты для бота {user.Username}");

                await client.ReceiveAsync(
                    updateHandler: handler,
                    cancellationToken: stoppingToken
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении обновлений: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}