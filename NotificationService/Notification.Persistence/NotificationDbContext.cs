using Microsoft.EntityFrameworkCore;
using Notification.Domain.Entities.User;
using Notification.Persistence.Configurations;

namespace Notification.Persistence;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<UserState> UserStates { get; set; }
    public DbSet<UserData> UserDatas { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigureUserData).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigureUserState).Assembly);
    }
}