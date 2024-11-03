using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderManagementSystem.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired();
        builder.Property(f => f.Description).IsRequired();
        builder.Property(f => f.Quantity).IsRequired();
        builder.Property(f => f.Price).IsRequired();
    }
}