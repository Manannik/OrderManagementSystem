using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka;

public static class Extensions
{
    public static IServiceCollection AddConsumer<TMessage, THandler>(this IServiceCollection serviceCollection,
        IConfigurationSection configurationSection) where THandler : class, IMessageHandler<TMessage>
    {
        serviceCollection.Configure<KafkaSetting>(configurationSection);
        serviceCollection.AddHostedService<KafkaConsumer<TMessage>>();
        serviceCollection.AddScoped<IMessageHandler<TMessage>, THandler>();

        return serviceCollection;
    }
}