using Application.Models;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using WebApplication.Controllers.Validators;

namespace WebApplication.Extensions
{
    public static class WebExtensions
    {
        public static IServiceCollection AddWeb(
            this IServiceCollection services)
    {
        services.AddControllers();
        services.AddTransient<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        services.AddTransient<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
        services.AddTransient<IValidator<OrderedQuantity>, UpdateProductQuantityRequestValidator>();
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        return services;
    }
    }
}