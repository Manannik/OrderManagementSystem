using Microsoft.Extensions.DependencyInjection;
using Notification.Infrastructure.Extensions;
using Notification.Infrastructure.Telegram.Attributes;
using Notification.Infrastructure.Telegram.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Notification.Infrastructure.Telegram.Pages
{
    [Page("OrderStatusPage")]
    public class OrderStatusPage(IServiceProvider services) : IPage
    {
        public Task<PageResult> Handle(Update update, UserStateModel userStateModel)
        {
            if (update.CallbackQuery.Data == "Ввести новый номер заказа")
            {
                userStateModel.Pages.Pop();
                return userStateModel.CurrentPage.View(update, userStateModel);
            }
            return Task.FromResult(new PageResult(@"Нажми на кнопку - получишь в результат
И твоя мечта осуществится
Нажми на кнопку, но что же ты не рад
Тебе больше не к чему стремиться", GetReplyKeyboard())
            {
                UpdatedUserStateModel = userStateModel
            });
        }

        public Task<PageResult> View(Update update, UserStateModel userStateModel)
        {
            var text = $@"Статус твоего заказа {userStateModel.OrderStatus.GetDisplayName()}";
            return Task.FromResult(new PageResult(text,GetReplyKeyboard())
            {
                UpdatedUserStateModel = userStateModel
            });
        }

        private ReplyMarkup GetReplyKeyboard()
        {
            var keyboardButtons = new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Ввести номер заказа"),
                },
            };

            var replyMarkup = new InlineKeyboardMarkup(keyboardButtons);

            return replyMarkup;
        }
    }
}
