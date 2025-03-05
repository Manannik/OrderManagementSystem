using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities.User;
using Notification.Persistence.Extensions;

namespace Notification.Persistence.Configurations;

public class ConfigureUserState : IEntityTypeConfiguration<UserState>
{
    public void Configure(EntityTypeBuilder<UserState> builder)
    {
        builder.ToTable("user_state");

        builder.HasKey(us => us.TelegramUserId);

        builder.Property(us => us.LastUserMessage)
            .HasConversion(new UserMessageConverter());
        
        builder.Property(us => us.Pages).JsonValueObjectCollectionConversion();

        builder.Property(us => us.UserDataId).IsRequired(false);
    }
}