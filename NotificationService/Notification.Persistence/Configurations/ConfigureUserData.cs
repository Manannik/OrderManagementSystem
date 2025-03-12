using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Domain.Entities.User;

namespace Notification.Persistence.Configurations;

public class ConfigureUserData : IEntityTypeConfiguration<UserData>
{
    public void Configure(EntityTypeBuilder<UserData> builder)
    {
        builder.ToTable("user_data");

        builder.HasKey(u => u.OrderId);

        builder.HasOne(ud => ud.UserState)
            .WithOne(ud => ud.UserData)
            .HasForeignKey<UserState>(f=>f.UserDataId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}