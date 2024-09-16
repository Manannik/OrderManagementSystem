using WebApplication1.Middleware;

namespace WebApplication1.Extentions;

public static class WebExtentions
{
    public static IServiceCollection Web(
        this IServiceCollection services)
    {
        services.AddSingleton(typeof(CatalogServiceExceptionHandlerMiddleware));
        return services;
    }
}