using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderManagementSystem.Persistance;

public class CatalogDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(option =>
        {
            option.HasKey(f => f.Id);
        });

        modelBuilder.Entity<Category>(option =>
        {
            option.HasKey(f => f.Id);

            option.HasMany(f => f.Products)
                .WithOne(f => f.Category)
                .HasForeignKey(f => f.CategoryId);
        });

        var categories = new[]
        {
            new Category()
            {
                Id =Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651"),
                Name = "Одежда"
            }
        };
        modelBuilder.Entity<Category>().HasData(categories);
    }
}