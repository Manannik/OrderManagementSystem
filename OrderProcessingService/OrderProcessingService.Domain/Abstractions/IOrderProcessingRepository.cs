using OrderProcessingService.Domain.Entities;

namespace OrderProcessingService.Domain.Abstractions;

public interface IOrderProcessingRepository
{
    Task CreateAsync(ProcessingOrder processingOrder, CancellationToken ct);
    Task<ProcessingOrder> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ProcessingOrder> ChangeProcessingOrderStatusToProcessing(ProcessingOrder processingOrder, CancellationToken ct);
    Task ChangeProcessingOrderStatusToCompleted(ProcessingOrder processingOrder, CancellationToken ct);
}