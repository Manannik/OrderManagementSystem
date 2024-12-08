using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using Order.Persistence.Configurations;

namespace Order.Persistence
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<Domain.Entities.Order> Orders { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new ProductItemConfiguration());
    }
    }
}