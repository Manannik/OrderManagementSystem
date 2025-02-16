using OrderProcessingService.Application.Models;

namespace OrderProcessingService.Application.Abstractions;

public interface IOrderProcessingService
{
    public Task<ProcessingOrderModel> AssembleOrderAsync(Guid id,CancellationToken ct);
    public Task<List<DeliveryOrderModel>> TakeOrdersForDeliveryAsync(List<Guid> guids,CancellationToken ct);
}