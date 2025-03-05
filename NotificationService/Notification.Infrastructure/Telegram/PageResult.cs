using Notification.Infrastructure.Telegram.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace Notification.Infrastructure.Telegram
{
    public class PageResult
    {
        public string Text { get; }
        public ReplyMarkup? ReplyMarkup { get; }
        public UserStateModel UpdatedUserStateModel { get; set; }

        public PageResult(string text, ReplyMarkup? replyMarkup)
        {
            Text = text;
            ReplyMarkup = replyMarkup;
            UpdatedUserStateModel = new UserStateModel();
        }

        public PageResult(string text)
        {
            Text = text;
            ReplyMarkup = null;
            UpdatedUserStateModel = new UserStateModel();
        }
    }
}