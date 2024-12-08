using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingService.Domain.Abstractions;

namespace Infrastructure.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IOrderProcessingRepository, OrderProcessingRepository>();

        services.AddDbContext<OrderProcessingDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("OrderProcessingConnectionString"));
        });
        return services;
    }
}