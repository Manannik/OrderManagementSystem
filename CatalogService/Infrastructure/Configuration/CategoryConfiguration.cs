using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderManagementSystem.Infrastructure.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).IsRequired();
        builder.HasMany(f => f.Products)
            .WithMany(f => f.Categories)
            .UsingEntity<ProductCategory>(
                f => f.HasOne(g => g.Product)
                    .WithMany()
                    .HasForeignKey(j => j.ProductId),
                g => g.HasOne(h => h.Category)
                    .WithMany()
                    .HasForeignKey(f => f.CategoryId),
                f => { f.HasKey(g => new { g.ProductId, g.CategoryId }); });

        var categories = new[]
        {
            new Category()
            {
                Id =Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651"),
                Name = "Одежда"
            },
            new Category() {
                Id =Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d652"),
                Name = "Джинсы"
            },
            new Category() {
                Id =Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d653"),
                Name = "Куртка"
            },
        };
        builder.HasData(categories);
    }
}