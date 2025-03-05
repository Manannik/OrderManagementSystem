using Notification.Infrastructure.Telegram.Pages;

namespace Notification.Infrastructure.Telegram.Models;

public class UserStateModel
{
    public long TelegramUserId { get; set; }
    public int? LastMessageId { get; set; }
    public OrderStatusModel OrderStatus {get;set;}
    public Stack<IPage> Pages { get; set; } = new();
    public IPage CurrentPage => Pages.Peek();
    public void AddPage(IPage page)
    {
        if (CurrentPage.GetType() != page.GetType())
        {
            Pages.Push(page);
        }
    }
}