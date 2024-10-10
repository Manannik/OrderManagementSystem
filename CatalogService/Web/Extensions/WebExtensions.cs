using Application.Models;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using WebApplication1.Controllers.Validators;

namespace WebApplication1.Extensions;

public static class WebExtensions
{
    public static IServiceCollection AddWeb(
        this IServiceCollection services)
    {
        //services.AddTransient<IRequestPreProcessor<CreateProductRequest>, CreateProductRequestPreProcessor>();
        services.AddControllers();
        services.AddTransient<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        services.AddTransient<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
        services.AddTransient<IValidator<OrderedQuantity>, UpdateProductQuantityRequestValidator>();
        
        //services.AddTransient<IRequestPreProcessor<CreateProductRequest>, CreateProductRequestPreProcessor>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        return services;
    }
}