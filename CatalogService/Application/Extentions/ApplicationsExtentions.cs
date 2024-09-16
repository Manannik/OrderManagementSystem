using Application.BusinessLogic.Commands.CreateProduct;
using Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extentions;

public static class ApplicationsExtentions
{
    public static IServiceCollection Application(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
        });
        return services;
    }
}