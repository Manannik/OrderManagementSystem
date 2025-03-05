using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Domain.Abstractions;
using Notification.Persistence.Repositories;

namespace Notification.Persistence.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddDbContext<NotificationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("NotificationServiceLocalConnectionString"));
        });
        
        return services;
    }
}