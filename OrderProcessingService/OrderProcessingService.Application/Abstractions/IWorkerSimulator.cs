using OrderProcessingService.Application.Models;

namespace OrderProcessingService.Application.Abstractions;

public interface IWorkerSimulator
{
    Task ProcessOrderInWarehouseAsync(ProcessingOrderModel processingOrderModel);
    Task TransferOrderToDelivery(List<Guid> processingOrderIds);
}