using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Domain.Entities;

namespace Infrastructure.Persistence;

public class OrderProcessingDbContext(DbContextOptions<OrderProcessingDbContext> options) : DbContext(options)
{
    public DbSet<ProcessingOrder> ProcessingOrders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}