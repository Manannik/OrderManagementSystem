using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Entities;

namespace Order.Persistence.Configurations
{
    public class ProductItemConfiguration : IEntityTypeConfiguration<ProductItem>
    {
        public void Configure(EntityTypeBuilder<ProductItem> builder)
    {
        builder.HasKey(f => f.ProductId);
        builder.Property(f => f.Price).IsRequired();
        builder.Property(f => f.Quantity).IsRequired();
    }
    }
}