namespace Notification.Domain.Entities.User;

public class UserState
{
    public long TelegramUserId { get; set; }
    public UserMessage? LastUserMessage { get; set; }
    public List<string> Pages { get; set; } = new();
    public Guid? UserDataId { get; set; }
    public UserData? UserData { get; set; }
}