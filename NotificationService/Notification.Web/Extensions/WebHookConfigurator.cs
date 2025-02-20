using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Common.Configuration;

namespace Notification.Web.Extensions
{
    public class WebHookConfigurator(
        ITelegramBotClient telegramBotClient,
        IOptions<BotConfiguration> botConfiguration) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var webhookAddress = botConfiguration.Value.HostAddress + BotConfiguration.UpdateRoute;
            await telegramBotClient.SetWebhook(
                url: webhookAddress, 
                secretToken: botConfiguration.Value.SecretToken,
                cancellationToken: cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await telegramBotClient.DeleteWebhook(cancellationToken: cancellationToken);
        }
    }
}
