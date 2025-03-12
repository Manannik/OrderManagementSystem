using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Notification.Infrastructure.Telegram.Attributes;
using Notification.Infrastructure.Telegram.Pages;

namespace Notification.Infrastructure.Telegram;

public class PagesFactory
{
    private readonly IServiceProvider _services;
    private readonly Dictionary<string, Type> _pageTypes;

    public PagesFactory(IServiceProvider services)
    {
        _services = services;

        var assembly = typeof(PagesFactory).Assembly;
        _pageTypes = assembly.GetTypes()
            .Where(type => type.GetCustomAttribute<PageAttribute>() != null)
            .ToDictionary(
                type => type.GetCustomAttribute<PageAttribute>()!.TypeName,
                type => type
            );
    }

    public IPage GetPage(string typeName)
    {
        if (!_pageTypes.TryGetValue(typeName, out var type))
        {
            throw new Exception($"Страница с типом '{typeName}' не найдена.");
        }
        return (IPage)_services.GetRequiredService(type);
    }
}