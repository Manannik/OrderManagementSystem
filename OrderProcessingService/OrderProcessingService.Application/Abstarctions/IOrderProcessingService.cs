using OrderProcessingService.Application.Models;

namespace OrderProcessingService.Application.Abstarctions;

public interface IOrderProcessingService
{
    public Task<ProcessingOrderModel> ProcessOrderByIdAsync(Guid id,CancellationToken ct);
    public Task UpdateProcessingOrderStatusToCompletedAsync(Guid id, List<ProcessingOrderItemModel> items, CancellationToken ct);
}