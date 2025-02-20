using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Common.User;

namespace Telegram.Common.Pages
{
    public class PageResult
    {
        public string Text { get;}
        public ReplyMarkup ReplyMarkup { get;}
        public UserState UpdatedUserState { get; set; }
        public PageResult(string text, ReplyMarkup replyMarkup)
        {
            Text = text;
            ReplyMarkup = replyMarkup;
        }
    }
}