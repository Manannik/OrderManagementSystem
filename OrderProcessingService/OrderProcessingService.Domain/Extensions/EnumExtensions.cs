using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace OrderProcessingService.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var displayAttribute = value.GetType()
            .GetField(value.ToString())
            ?.GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? value.ToString();
    }
}