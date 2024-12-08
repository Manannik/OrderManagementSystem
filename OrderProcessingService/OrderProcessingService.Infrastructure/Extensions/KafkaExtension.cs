using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Application.Abstarctions;
using OrderProcessingService.Infrastructure.Kafka;

namespace OrderProcessingService.Infrastructure.Extensions;

public static class KafkaExtension
{
    public static void AddConsumer<TMessage>(
        this IServiceCollection services,
        IConfiguration configurationSection)
    {
        //services.Configure<KafkaSettings>(configurationSection);
        services.AddSingleton<IKafkaConsumer<TMessage>, IKafkaConsumer<TMessage>>();
    }
}