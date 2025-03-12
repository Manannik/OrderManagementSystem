using Messaging.Kafka;
using Messaging.Kafka.Models;

namespace Notification.Web.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddWeb<TMessage>(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConsumerConfig = configuration.GetSection("Kafka:Notification");
        services.AddConsumer<NotificationKafkaModel, OrderNotificationMessageHandler>(kafkaConsumerConfig);

        return services;
    }
}