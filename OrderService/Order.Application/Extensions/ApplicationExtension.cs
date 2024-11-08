using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Abstractions;
using Order.Application.Services;
using Order.Infrastructure.Services;

namespace Order.Application.Extensions;

public static class ApplicationExtension
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}