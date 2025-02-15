
using Messaging.Kafka;
using Messaging.Kafka.Models;
using Microsoft.Extensions.Options;
using Serilog;

namespace OrderProcessingService.Web.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddWeb<TMessage>(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConfig = configuration.GetSection("Kafka:OrderCreated");
        services.AddConsumer<OrderCreated, OrderCreatedMessageHandler>(kafkaConfig);
        return services;
    }
}