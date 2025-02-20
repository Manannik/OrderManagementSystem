using Microsoft.Extensions.DependencyInjection;
using Telegram.Common.Pages;

namespace Telegram.Common.Firebase
{
    public class PagesFactory(IServiceProvider services)
    {
        public IPage GetPage(string typeName)
        {
            var type = Type.GetType("TelegramBot.Common.Pages." + typeName) ?? throw new Exception("Такого нет в проекте");
            return (IPage)services.GetRequiredService(type);
        }
    }
}
