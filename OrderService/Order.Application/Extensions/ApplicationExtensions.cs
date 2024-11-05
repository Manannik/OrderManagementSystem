using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Abstractions;
using Order.Application.Services;

namespace Order.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICatalogServiceClient, CatalogServiceClient>();
        return services;
    }
}