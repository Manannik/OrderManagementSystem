using Notification.Infrastructure.Telegram.Models;
using Telegram.Bot.Types;

namespace Notification.Infrastructure.Telegram.Pages
{
    public interface IPage
    {
        Task<PageResult> View(Update update, UserStateModel userStateModel);
        Task<PageResult> Handle(Update update, UserStateModel userStateModel);
    }
}
