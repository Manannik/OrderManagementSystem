using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Infrastructure.Repository;

namespace OrderManagementSystem.Infrastructure.Extentions;

public static class PersistanceExtentions
{
    public static IServiceCollection Persistance(
        this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IProductRepository, ProductRepositoryy>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("CatalogServiceConnectionString"));
        });
        return services;
    }
}
