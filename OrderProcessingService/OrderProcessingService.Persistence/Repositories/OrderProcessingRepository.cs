using OrderProcessingService.Domain.Abstractions;
using OrderProcessingService.Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class OrderProcessingRepository(OrderProcessingDbContext dbContext) : IOrderProcessingRepository
{
    public async Task CreateAsync(ProcessingOrder processingOrder, CancellationToken ct)
    {
        await dbContext.ProcessingOrders.AddAsync(processingOrder, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}