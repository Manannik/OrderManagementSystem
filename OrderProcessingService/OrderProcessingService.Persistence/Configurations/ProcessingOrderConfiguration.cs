using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingService.Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public class ProcessingOrderConfiguration : IEntityTypeConfiguration<ProcessingOrder>
{
    public void Configure(EntityTypeBuilder<ProcessingOrder> modelBuilder)
    {
        modelBuilder.HasKey(f => f.Id);
        modelBuilder.Property(f => f.OrderId).IsRequired();
        modelBuilder.HasMany(f => f.Items).WithOne(f => f.ProcessingOrder).HasForeignKey(f => f.ProcessingOrderId);
        modelBuilder.Property(f => f.CreatedAt).IsRequired();
        modelBuilder.Property(f => f.Stage).IsRequired();
        modelBuilder.Property(f => f.Status).IsRequired();
        modelBuilder.Property(f => f.TrackingNumber).IsRequired();
    }
}