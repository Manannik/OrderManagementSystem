using OrderProcessingService.Application.Models;

namespace OrderProcessingService.Application.Abstractions;

public interface IOrderProcessingService
{
    public Task<ProcessingOrderModel> ProcessOrderByIdAsync(Guid id,CancellationToken ct);
}