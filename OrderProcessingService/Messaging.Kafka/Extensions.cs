using Messaging.Kafka.Consumer;
using Messaging.Kafka.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka;

public static class Extensions
{
    public static IServiceCollection AddConsumer<TMessage, THandler>(this IServiceCollection serviceCollection,
        IConfigurationSection configurationSection) where THandler : class, IMessageHandler<TMessage>
    {
        serviceCollection.Configure<OrderCreatedKafkaSettings>(configurationSection);
        serviceCollection.AddHostedService<KafkaConsumer<TMessage>>();
        serviceCollection.AddScoped<IMessageHandler<TMessage>, THandler>();

        return serviceCollection;
    }
    
    public static void AddProducer<TMessage>(
        this IServiceCollection services,
        IConfiguration configurationSection)
    {
        services.Configure<NotificationKafkaSettings>(configurationSection);
        services.AddSingleton<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>();
    }
}