namespace Notification.Infrastructure.Telegram;

public class BotConfiguration
{
    public const string SectionName = "BotConfiguration";
    public const string UpdateRoute = "/webhook/update";

    public string BotToken { get; set; }
    public string HostAdress { get; set; }
    public string SecretToken { get; set; }
}