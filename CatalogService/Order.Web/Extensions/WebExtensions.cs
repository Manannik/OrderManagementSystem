using FluentValidation;
using Order.Application.Models;
using Order.Web.Controllers.Validators;

namespace Order.Web.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddTransient<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();

        return services;
    }
}