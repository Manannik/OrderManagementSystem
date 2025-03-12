using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Telegram;
using Notification.Infrastructure.Telegram.Pages;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Notification.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUpdateHandler, UpdateHandler>();
        var botConfiguration = configuration.GetSection(BotConfiguration.SectionName);
        
        services.AddHttpClient("tgBotClient").AddTypedClient<ITelegramBotClient>((httpclient, services) =>
        {
            var botConfig = services.GetService<IOptions<BotConfiguration>>()!.Value;
            var options = new TelegramBotClientOptions(botConfig.BotToken);
            return new TelegramBotClient(options, httpclient);
        });
        
        services.AddHostedService<TelegramLongPollingService>();
        services.Configure<BotConfiguration>(botConfiguration);
        
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes().Where(f => typeof(IPage).IsAssignableFrom(f) && !f.IsAbstract);
        foreach (var type in types)
        {
            services.AddScoped(type);
        }
        services.AddScoped<PagesFactory>();
        
        return services;
    }
}