
namespace Notification.Domain.Entities.User
{
    public class UserData
    {
        public Guid OrderId { get; set; }
        public UserState? UserState { get; set; }
        public Stage Stage { get; set; }
    }
}
