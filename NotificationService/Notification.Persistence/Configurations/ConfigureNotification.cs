using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notification.Persistence.Configurations;

public class ConfigureNotification : IEntityTypeConfiguration<Domain.Entities.Notification>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.OrderId);

        builder.Property(n => n.OrderId).HasColumnName("order_id");
        builder.Property(n => n.ChatId).HasColumnName("chat_id");

        builder.Property(n => n.Status)
            .HasColumnName("notification_status")
            .HasColumnType("status");
    }
}