using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Common.User;

namespace Telegram.Common.Pages
{
    public class OrderStatusPage(IServiceProvider services) : IPage
    {
        public PageResult Handle(Update update, UserState userState)
        {
            if (update.CallbackQuery.Data == "Пупупу")
            {
                return services.GetRequiredService<OrderStatusPage>().View(update, userState);
            }

            return new PageResult(@"Нажми на кнопку - получишь в результат
И твоя мечта осуществится
Нажми на кнопку, но что же ты не рад
Тебе больше не к чему стремиться", GetReplyKeyboard());
        }

        public PageResult View(Update update, UserState userState)
        {
            var text = @"пупупу";
            return new PageResult(text);
            /*
            return new PageResult(text, GetReplyKeyboard())
            {
                UpdatedUserState = userState
            };
            */
        }

        private ReplyMarkup GetReplyKeyboard()
        {
            var keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("пупупу"),
                },
            };

            var replyMarkup = new InlineKeyboardMarkup(keyboardButtons);

            return replyMarkup;
        }
    }
}
