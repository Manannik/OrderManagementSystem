using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Application.Abstarctions;
using OrderProcessingService.Application.Services;

namespace OrderProcessingService.Application.Extensions;

public static class ApplicationExtension
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddHangfire((sp,config) =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>()
                .GetConnectionString("HangfireConnectionString");
            
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString);
        });
        services.AddScoped<IOrderProcessingService,Services.OrderProcessingService>();
        services.AddScoped<IHangfireBackgroundTaskService,HangfireBackgroundTaskService>();
        
        services.AddHangfireServer();
    }
}