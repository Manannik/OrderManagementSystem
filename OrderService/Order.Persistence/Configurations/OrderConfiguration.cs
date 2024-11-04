using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Domain.Entities.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Order> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Cost).IsRequired();
        builder.Property(f => f.OrderStatus).IsRequired();
        builder.HasMany(f => f.ProductItems)
               .WithMany(f => f.Orders);
    }
}
