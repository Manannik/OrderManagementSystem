using OrderProcessingService.Application.Models;
using OrderProcessingService.Domain.Entities;

namespace OrderProcessingService.Application.Abstractions;

public interface IWorkerSimulator
{
    Task SimulateAsync(CancellationToken ct, ProcessingOrderModel processingOrderModel, ProcessingOrder processingOrder);
}