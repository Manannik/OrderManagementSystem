using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingService.Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public class ItemConfiguration:IEntityTypeConfiguration<ProcessingOrderItem>
{
    public void Configure(EntityTypeBuilder<ProcessingOrderItem> modelBuilder)
    {
        modelBuilder.HasKey(f => f.ProductId);
        modelBuilder.Property(f => f.Quantity).IsRequired();
        modelBuilder.Property(f => f.ProcessingOrderItemStatus).IsRequired();
        modelBuilder.HasOne(f => f.ProcessingOrder).WithMany(f => f.Items).HasForeignKey(f => f.ProcessingOrderId);
    }
}