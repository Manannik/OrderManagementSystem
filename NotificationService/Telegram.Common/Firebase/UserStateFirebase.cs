using Telegram.Common.User;

namespace Telegram.Common.Firebase
{
    public class UserStateFirebase
    {
        public UserData UserData { get; set; }
        public List<string> PagesNames { get; set; }
    }
}
