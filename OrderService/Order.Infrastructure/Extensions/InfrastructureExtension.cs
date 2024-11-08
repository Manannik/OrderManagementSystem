using Microsoft.Extensions.DependencyInjection;
using Order.Application.Abstractions;
using Order.Infrastructure.Kafka;
using Order.Infrastructure.Services;

namespace Order.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<ICatalogServiceClient, CatalogServiceClient>();
        services.AddScoped<IKafkaProducer, KafkaProducer>();
        return services;
    }
}