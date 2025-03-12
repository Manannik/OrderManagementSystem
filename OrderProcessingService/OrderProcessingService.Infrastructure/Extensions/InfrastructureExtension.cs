using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Application.Abstractions;
using OrderProcessingService.Infrastructure.Services;

namespace OrderProcessingService.Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static void AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddHangfire((sp, config) =>
        {
            var connectionString = sp.GetRequiredService<IConfiguration>()
                .GetConnectionString("HangfireConnectionString");

            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString));
        });

        services.AddHangfireServer();
        services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();
    }
}