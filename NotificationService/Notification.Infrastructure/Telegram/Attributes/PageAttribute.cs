namespace Notification.Infrastructure.Telegram.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PageAttribute : Attribute
{
    public string TypeName { get; }

    public PageAttribute(string typeName)
    {
        TypeName = typeName;
    }
}