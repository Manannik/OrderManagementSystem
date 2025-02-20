namespace Telegram.Common.User
{
    public class UserData
    {
        public string? TrackingNumber { get; set; }
        public Message? LastMessage { get; set; }
        public override string ToString()
        {
            return TrackingNumber;
        }
    }
}
