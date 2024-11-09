using Microsoft.Extensions.DependencyInjection;
using Order.Application.Abstractions;
using Order.Application.Services;

namespace Order.Application.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
    }
}