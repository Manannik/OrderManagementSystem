using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Abstractions;
using Order.Application.Models;
using Order.Infrastructure.Services;

namespace Order.Infrastructure.Extensions
{
    public static class InfrastructureExtension
    {
        public static void AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<ICatalogServiceClient, CatalogServiceClient>();
            services.AddProducer<CreateOrderKafkaModel>(configuration.GetSection("Kafka:Order"));
            services.AddProducer<UpdatedOrderKafkaModel>(configuration.GetSection("Kafka:UpdatedOrder"));
            services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(options =>
            {
                var catalogServiceUrl = configuration["ServiceUrls:CatalogService"];
                options.BaseAddress = new Uri(catalogServiceUrl);
            });
        }
    }
}