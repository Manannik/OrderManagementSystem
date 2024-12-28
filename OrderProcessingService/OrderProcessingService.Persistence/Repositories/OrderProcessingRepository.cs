using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Domain.Abstractions;
using OrderProcessingService.Domain.Entities;
using OrderProcessingService.Domain.Enums;

namespace Infrastructure.Persistence.Repositories;

public class OrderProcessingRepository(OrderProcessingDbContext dbContext) : IOrderProcessingRepository
{
    public async Task CreateAsync(ProcessingOrder processingOrder, CancellationToken ct)
    {
        processingOrder.Stage = Stage.Assembly;
        processingOrder.Status = ProcessingOrderStatus.New;
        processingOrder.CreatedAt = DateTime.UtcNow;
        
        await dbContext.ProcessingOrders.AddAsync(processingOrder, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<ProcessingOrder> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await dbContext.ProcessingOrders.Include(f=>f.Items).FirstOrDefaultAsync(f=>f.Id == id,ct);
    }

    public async Task<ProcessingOrder> ChangeProcessingOrderStatusToProcessing(ProcessingOrder processingOrder, CancellationToken ct)
    {
        processingOrder.Status = ProcessingOrderStatus.Processing;
        dbContext.ProcessingOrders.Update(processingOrder);
        await dbContext.SaveChangesAsync(ct);
        return processingOrder;
    }

    public async Task ChangeProcessingOrderStatusToCompleted(ProcessingOrder processingOrder, CancellationToken ct)
    {
        processingOrder.Status = ProcessingOrderStatus.Completed;
        dbContext.ProcessingOrders.Update(processingOrder);
        await dbContext.SaveChangesAsync(ct);
    }
}