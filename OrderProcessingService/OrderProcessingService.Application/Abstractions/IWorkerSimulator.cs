using OrderProcessingService.Application.Models;
using OrderProcessingService.Domain.Entities;

namespace OrderProcessingService.Application.Abstractions;

public interface IWorkerSimulator
{
    Task SimulateAsync(ProcessingOrderModel processingOrderModel);
}