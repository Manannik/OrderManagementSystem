using Application.BusinessLogic.Commands.CreateProduct;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ApplicationsExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
        });
        return services;
    }
}