using Telegram.Bot.Types;
using Telegram.Common.User;

namespace Telegram.Common.Pages
{
    public interface IPage
    {
        PageResult View(Update update, UserState userState);
        PageResult Handle(Update update, UserState userState);
    }
}
