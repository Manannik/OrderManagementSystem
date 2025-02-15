using System.Reflection;
using FluentValidation;

namespace WebApplication.Extensions
{
    public static class WebExtensions
    {
        public static IServiceCollection AddWeb(
            this IServiceCollection services)
        {
            services.AddControllers();
            //services.AddScoped<IValidator<CreateProductRequest>, CreateProductCommandValidator>();
            //services.AddScoped<IValidator<UpdateProductRequest>, UpdateProductRequestValidator>();
            //services.AddScoped<IValidator<OrderedQuantity>, UpdateProductQuantityRequestValidator>();
            
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            
            //services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));

            return services;
        }
    }
}