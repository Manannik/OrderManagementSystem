using System.Reflection;
using Firebase.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Common.Configuration;
using Telegram.Common.Firebase;
using Telegram.Common.Pages;
using Telegram.Common.Storage;

namespace Telegram.Common
{
    public static class ContainerConfigurator
    {
        public static void Configure(IConfiguration configuration, IServiceCollection services)
        {
            var firebaseConfigurationSection = configuration.GetSection(FirebaseConfiguration.SectionName);
            services.Configure<FirebaseConfiguration>(firebaseConfigurationSection);

            var botConfigurationSection = configuration.GetSection(BotConfiguration.SectionName);
            services.Configure<BotConfiguration>(botConfigurationSection);

            services.AddSingleton<UserStateStorage>();
            services.AddSingleton<FirebaseProvider>();
            services.AddSingleton(services =>
            {
                var firebaseConfig = services.GetService<IOptions<FirebaseConfiguration>>()!.Value;

                return new FirebaseClient(firebaseConfig.BasePath, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(firebaseConfig.Secret)
                });
            });

            services.AddHttpClient("tgBotClient").AddTypedClient<ITelegramBotClient>((httpclient, services) =>
            {
                var botConfig = services.GetService<IOptions<BotConfiguration>>()!.Value;
                var options = new TelegramBotClientOptions(botConfig.BotToken);
                return new TelegramBotClient(options, httpclient);
            });

            services.AddScoped<IUpdateHandler, UpdateHandler>();

            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes().Where(f => typeof(IPage).IsAssignableFrom(f) && !f.IsAbstract);
            foreach (var type in types)
            {
                services.AddSingleton(type);
            }

            services.AddSingleton<PagesFactory>();
        }
    }
}
