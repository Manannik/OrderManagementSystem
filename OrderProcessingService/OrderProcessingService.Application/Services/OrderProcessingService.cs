using Microsoft.Extensions.Logging;
using OrderProcessingService.Application.Abstarctions;
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
    private IHangfireBackgroundTaskService _hangfireBackgroundTaskService;

    public OrderProcessingService(ILogger<OrderProcessingService> logger,
        IOrderProcessingRepository processingRepository,
        IHangfireBackgroundTaskService hangfireBackgroundTaskService)
    {
        _logger = logger;
        _processingRepository = processingRepository;
        _hangfireBackgroundTaskService = hangfireBackgroundTaskService;
    }

    public async Task<ProcessingOrderModel> ProcessOrderByIdAsync(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Запуск метода GetById для заказа с id: {Id}", id);

        var existingProcessingOrder = await _processingRepository.GetByIdAsync(id, ct);
        ValidateProcessingOrder(existingProcessingOrder, id);

        await _processingRepository.ChangeProcessingOrderStatusToProcessing(existingProcessingOrder, ct);

        var existingProcessingOrderModel = MapToModel(existingProcessingOrder);

        await SimulateEmployeeWork(ct, existingProcessingOrderModel, existingProcessingOrder);
        
        //кажется лишним ходить второй раз в БД после работы метода SimulateEmployeeWork
        var result = await _processingRepository.GetByIdAsync(id, ct);
        
        _logger.LogInformation("Успешное завершение метода GetById для заказа с id: {Id}", id);
        return MapToModel(result);
    }

    private async Task SimulateEmployeeWork(CancellationToken ct, ProcessingOrderModel existingProcessingOrderModel,
        ProcessingOrder existingProcessingOrder)
    {
        _logger.LogInformation("Начинаем сборку товара для задачи с ID: {TaskId}", existingProcessingOrderModel.Id);
        await Task.Delay(1000, ct);
        foreach (var item in existingProcessingOrderModel.Items)
        {
            _logger.LogInformation("Пришел за товаром {ProductId}", item.ProductId);
            item.ProcessingOrderItemStatus = ProcessingOrderItemStatusModel.Ready;
            _logger.LogInformation("Статус позиции {ProductId} изменен на Ready", item.ProductId);
        }
        await Task.Delay(1000, ct);
        _logger.LogInformation("Все позиции готовы. Меняем состояние сборки на Completed.");
        await UpdateProcessingOrderStatusToCompletedAsync(existingProcessingOrder.Id, existingProcessingOrderModel.Items, ct);
        
        await Task.Delay(1000, ct);
        _logger.LogInformation("Процесс сборки завершен для заказа с ID: {OrderId}", existingProcessingOrder.Id);
    }

    public async Task UpdateProcessingOrderStatusToCompletedAsync(Guid id, List<ProcessingOrderItemModel> items, CancellationToken ct)
    {
        var existingProcessingOrder = await _processingRepository.GetByIdAsync(id, ct);
        await _processingRepository.ChangeProcessingOrderStatusToCompleted(existingProcessingOrder, ct);
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