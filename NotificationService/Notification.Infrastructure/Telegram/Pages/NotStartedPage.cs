using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Abstractions;
using Notification.Infrastructure.Telegram.Attributes;
using Notification.Infrastructure.Telegram.Models;
using Telegram.Bot.Types;

namespace Notification.Infrastructure.Telegram.Pages
{
    [Page("NotStartedPage")]
    public class NotStartedPage(IServiceProvider services) : IPage
    {
        public Task<PageResult> View(Update update, UserStateModel userStateModel)
        {
            return null;
        }

        public async Task<PageResult> Handle(Update update, UserStateModel userStateModel)
        {
            return await services.GetRequiredService<StartPage>().View(update, userStateModel);
        }
    }
}
