using OrderProcessingService.Web.Services;

namespace OrderProcessingService.Web.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddWeb<TMessage>(this IServiceCollection services)
    {
        services.AddHostedService<KafkaConsumerBackgroundService<TMessage>>();
        return services;
    }
}