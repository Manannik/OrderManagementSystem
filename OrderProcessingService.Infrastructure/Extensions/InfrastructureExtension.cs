using Hangfire;
using Microsoft.AspNetCore.Builder;
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
        
        services.AddHangfire(configuration => 
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseSqlServerStorage("SQLConnectionString"));
        
        services.AddHangfireServer();
    }
    
    public static void UseInfrastructure(this IApplicationBuilder app, IBackgroundJobClient backgroundJobs)
    {
        app.UseHangfireDashboard();

        backgroundJobs.Enqueue(() => Console.WriteLine("Hello, Hangfire!"));
    }
}