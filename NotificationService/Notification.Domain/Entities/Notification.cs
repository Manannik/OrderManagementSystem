namespace Notification.Domain.Entities;

public class Notification
{
    public Guid OrderId { get; set; }
    public NotificationStatus NotificationStatus { get; set; }
    public long ChatId { get; set; }
}