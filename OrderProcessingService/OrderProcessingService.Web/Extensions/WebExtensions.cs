using Messaging.Kafka;
using Messaging.Kafka.Models;

namespace OrderProcessingService.Web.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddWeb<TMessage>(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaConsumerConfig = configuration.GetSection("Kafka:OrderCreated");
        services.AddConsumer<OrderCreated, OrderCreatedMessageHandler>(kafkaConsumerConfig);
        services.AddProducer<NotificationKafkaModel>(configuration.GetSection("Kafka:Notification"));

        return services;
    }
}