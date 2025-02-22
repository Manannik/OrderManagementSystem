namespace Telegram.Common.User
{
    public class UserData
    {
        public string? OrderId { get; set; }
        public Message? LastMessage { get; set; }
        public override string ToString()
        {
            return OrderId;
        }
    }
}
