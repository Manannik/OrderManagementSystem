using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Infrastructure.Repository;

namespace OrderManagementSystem.Infrastructure.Extensions;

public static class PersistanceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("CatalogServiceConnectionString"));
        });
        return services;
    }
}
