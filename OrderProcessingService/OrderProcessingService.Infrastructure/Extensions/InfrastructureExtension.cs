using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderProcessingService.Application.Models.Kafka;

namespace OrderProcessingService.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddConsumer<CreateOrderKafkaModel>(configuration.GetSection("Kafka:Order"));
        
        services.AddHangfire((sp,config) =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>()
                .GetConnectionString("HangfireConnectionString");
            
            config.UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString);
        });
        services.AddHangfireServer();
    }
}