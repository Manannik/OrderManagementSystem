using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Common.User;

namespace Telegram.Common.Pages
{
    public class NotStartedPage(IServiceProvider services) : IPage
    {
        public PageResult View(Update update, UserState userState)
        {
            return null;
        }

        public PageResult Handle(Update update, UserState userState)
        {
            return services.GetRequiredService<StartPage>().View(update, userState);
        }
    }
}
