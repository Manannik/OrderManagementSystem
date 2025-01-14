using Hangfire;
using Microsoft.Extensions.Logging;
using OrderProcessingService.Application.Abstractions;
using OrderProcessingService.Application.Enums;
using OrderProcessingService.Application.Models;
using OrderProcessingService.Domain.Abstractions;
using OrderProcessingService.Domain.Entities;
using OrderProcessingService.Domain.Enums;
using OrderProcessingService.Domain.Exceptions;

namespace OrderProcessingService.Application.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private ILogger<OrderProcessingService> _logger;
    private IOrderProcessingRepository _processingRepository;

    public OrderProcessingService(ILogger<OrderProcessingService> logger,
        IOrderProcessingRepository processingRepository)
        {
            _logger = logger;
            _processingRepository = processingRepository;
        }

    public async Task<ProcessingOrderModel> ProcessOrderByIdAsync(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Запуск метода GetById для заказа с id: {Id}", id);

        var existingProcessingOrder = await _processingRepository.GetByIdAsync(id, ct);
        ValidateProcessingOrder(existingProcessingOrder, id);

        await _processingRepository.ChangeProcessingOrderStatusToProcessing(existingProcessingOrder, ct);

        var existingProcessingOrderModel = MapToModel(existingProcessingOrder);

        BackgroundJob.Enqueue<IWorkerSimulator>(worker => worker.SimulateAsync(ct,existingProcessingOrderModel,existingProcessingOrder));
        var result = await _processingRepository.GetByIdAsync(id, ct);
        
        _logger.LogInformation("Успешное завершение метода GetById для заказа с id: {Id}", id);
        return MapToModel(result);
    }

    private void ValidateProcessingOrder(ProcessingOrder existingProcessingOrder, Guid id)
    {
        if (existingProcessingOrder == null)
        {
            throw new OrderProcessingDoesNotExistsException(id.ToString());
        }

        if (existingProcessingOrder.Status != ProcessingOrderStatus.New)
        {
            throw new ProcessingOrderStatusException(id.ToString(), existingProcessingOrder.Status);
        }

        if (existingProcessingOrder.Stage != Stage.Assembly)
        {
            throw new ProcessingOrderStageException(id.ToString(), existingProcessingOrder.Stage);
        }
    }
    
    private ProcessingOrderModel MapToModel(ProcessingOrder existingProcessingOrder)
    {
        return new ProcessingOrderModel()
        {
            Id = existingProcessingOrder.Id,
            OrderId = existingProcessingOrder.OrderId,
            CreatedAt = existingProcessingOrder.CreatedAt,
            UpdatedAt = existingProcessingOrder.UpdatedAt,
            Stage = (StageModel)existingProcessingOrder.Stage,
            Status = (ProcessingOrderStatusModel)existingProcessingOrder.Status,
            Items = existingProcessingOrder.Items.Select(f => new ProcessingOrderItemModel()
            {
                ProductId = f.ProductId,
                ProcessingOrderItemStatus = (ProcessingOrderItemStatusModel)f.ProcessingOrderItemStatus,
                Quantity = f.Quantity
            }).ToList()
        };
    }
    
    private ProcessingOrder MapToEntity(ProcessingOrderModel processingOrderModel)
    {
        return new ProcessingOrder()
        {
            Id = processingOrderModel.Id,
            OrderId = processingOrderModel.OrderId,
            CreatedAt = processingOrderModel.CreatedAt,
            UpdatedAt = processingOrderModel.UpdatedAt,
            Stage = (Stage)processingOrderModel.Stage,
            Status = (ProcessingOrderStatus)processingOrderModel.Status,
            Items = processingOrderModel.Items.Select(item => new ProcessingOrderItem()
            {
                ProductId = item.ProductId,
                ProcessingOrderItemStatus = (ProcessingOrderItemStatus)item.ProcessingOrderItemStatus,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}