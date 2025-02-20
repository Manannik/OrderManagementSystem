namespace Telegram.Common.Configuration
{
    public class FirebaseConfiguration
    {
        public const string SectionName = "Firebase";
        public string BasePath { get; set; }
        public string Secret { get; set; }
    }
}
