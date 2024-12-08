using FluentValidation;
using Order.Application.Models;
using Order.Application.Requests;
using Order.Web.Controllers.Validators;

namespace Order.Web.Extensions
{
    public static class WebExtensions
    {
        public static IServiceCollection AddWeb(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IValidator<CreateOrderRequest>, CreateOrderRequestValidator>();
            services.AddScoped<IValidator<ChangeOrderStatusRequest>, ChangeOrderStatusRequestValidator>();

            return services;
        }
    }
}