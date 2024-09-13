using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Infrastructure.Configuration;

namespace OrderManagementSystem.Infrastructure;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
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