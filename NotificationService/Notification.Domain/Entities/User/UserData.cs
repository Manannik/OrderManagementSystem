using Telegram.Common.Pages;

namespace Notification.Domain.Entities.User
{
    public class UserData
    {
        public Guid Id { get; set; }
        public string? OrderId { get; set; }
        public long? LastMessageId { get; set; }
        public List<IPage> Pages { get; set; } = new ();
    }
}
