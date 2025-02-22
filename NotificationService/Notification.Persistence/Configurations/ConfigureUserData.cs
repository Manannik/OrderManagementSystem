using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities.User;

namespace Notification.Persistence.Configurations;

public class ConfigureUserData : IEntityTypeConfiguration<UserData>
{
    public void Configure(EntityTypeBuilder<UserData> builder)
    {
        builder.ToTable("user_data");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.OrderId).IsRequired(false);
        builder.Property(u => u.LastMessageId).IsRequired(false);

        builder.OwnsMany(p => p.Pages, builder => { builder.ToJson(); });
    }
}