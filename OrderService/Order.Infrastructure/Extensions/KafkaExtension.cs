using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Abstractions;
using Order.Infrastructure.Kafka;
using Order.Infrastructure.Services;

namespace Order.Infrastructure.Extensions;

public static class KafkaExtension
{
    public static void AddProducer<TMessage>(
        this IServiceCollection services,
        IConfiguration configurationSection)
    {
        services.Configure<KafkaSettings>(configurationSection);
        services.AddSingleton<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>();
    }
}