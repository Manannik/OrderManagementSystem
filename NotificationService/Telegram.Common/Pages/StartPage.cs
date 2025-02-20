using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Common.User;

namespace Telegram.Common.Pages
{
    public class StartPage(IServiceProvider services) : IPage
    {
        public PageResult Handle(Update update, UserState userState)
        {
            if (update?.CallbackQuery?.Data == "Проверить заказ по трек-номеру")
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
            var text = @"Привет я отправляю уведомления пользователям
учебного проекта Order Management System.
Если хочешь проверить статус заказа жми кнопку Проверить заказ по трек-номеру";
            userState.AddPage(this);
            return new PageResult(text, GetReplyKeyboard())
            {
                UpdatedUserState = userState
            };
        }

        private ReplyMarkup GetReplyKeyboard()
        {
            var keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Проверить заказ по трек-номеру"),
                },
            };

            var replyMarkup = new InlineKeyboardMarkup(keyboardButtons);

            return replyMarkup;
        }
    }
}
