using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Abstractions;
using Notification.Domain.Entities.User;
using Notification.Infrastructure.Telegram.Attributes;
using Notification.Infrastructure.Telegram.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Notification.Infrastructure.Telegram.Pages
{
    [Page("StartPage")]
    public class StartPage(IServiceProvider services, INotificationRepository notificationRepository) : IPage
    {
        public async Task<PageResult> Handle(Update update, UserStateModel userStateModel)
        {
            var userInput = update?.Message?.Text;

            if (userInput == null)
            {
                return await View(update, userStateModel);
            }
            
            var isGuid = Guid.TryParse(userInput, out Guid orderId);

            if (!isGuid)
            {
                return new PageResult(@"Вы неправильно ввели номер заказа
Введите заново", GetReplyKeyboard())
                {
                    UpdatedUserStateModel = userStateModel
                };
            }
            
            var userData = await notificationRepository.TryGetUserDataByOrderIdAsync(orderId, cancellationToken: default);
            
            if (userData != null)
            {
                var pages = userStateModel.Pages.Select(page => page.GetType().Name).ToList();
                
                var updatedUserState = new UserState
                {
                    TelegramUserId = userStateModel.TelegramUserId,
                    Pages = pages,
                    UserDataId = orderId
                };
                userData.UserState = updatedUserState;

                await notificationRepository.UpdateUserDataAsync(userData, cancellationToken: default);

                userStateModel.OrderStatus = (OrderStatusModel)userData.Stage;
                
                return await services.GetRequiredService<OrderStatusPage>().View(update, userStateModel);
            }
            
            return new PageResult(@"Вы неправильно ввели номер заказа
Введите заново", GetReplyKeyboard())
            {
                UpdatedUserStateModel = userStateModel
            };
        }

        public Task<PageResult> View(Update update, UserStateModel userStateModel)
        {
            var text = @"Привет я отправляю уведомления пользователям
учебного проекта Order Management System.
Если хочешь проверить статус заказа введи номер своего заказа";
            userStateModel.AddPage(this);
            return Task.FromResult(new PageResult(text)
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
                    InlineKeyboardButton.WithCallbackData("Проверить заказ по трек-номеру"),
                },
            };

            var replyMarkup = new InlineKeyboardMarkup(keyboardButtons);

            return replyMarkup;
        }
    }
}
