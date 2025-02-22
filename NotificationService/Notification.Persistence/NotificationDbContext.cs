using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities.User;
using Notification.Persistence.Configurations;

namespace Notification.Persistence;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Notification> Notifications { get; set; }
    public DbSet<UserData> UserData { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigureNotification).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigureUserData).Assembly);
    }
}