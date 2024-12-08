using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Application.Models.Kafka;

namespace OrderProcessingService.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddConsumer<CreateOrderKafkaModel>(configuration.GetSection("Kafka:Order"));
    }
}