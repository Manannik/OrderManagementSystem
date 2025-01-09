using System.Reflection;
using Application.BusinessLogic.Commands.CreateProduct;
using FluentValidation;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class ApplicationsExtensions
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
                configuration.AutoRegisterRequestProcessors = true;
                configuration.AddOpenBehavior(typeof(RequestPreProcessorBehavior<,>));
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}