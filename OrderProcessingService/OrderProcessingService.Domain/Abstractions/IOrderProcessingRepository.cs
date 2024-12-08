using OrderProcessingService.Domain.Entities;

namespace OrderProcessingService.Domain.Abstractions;

public interface IOrderProcessingRepository
{
    Task CreateAsync(ProcessingOrder processingOrder, CancellationToken ct);
}