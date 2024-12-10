using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Domain.Entities;

namespace Infrastructure.Persistence;

public class OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options) : DbContext(options)
{
    public DbSet<ProcessingOrder> ProcessingOrders { get; set; }
    public DbSet<Item> Items { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProcessingOrderConfiguration());
        modelBuilder.ApplyConfiguration(new ItemConfiguration());
    }
}