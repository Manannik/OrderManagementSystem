using Hangfire;
using OrderProcessingService.Application.Abstractions;
using OrderProcessingService.Application.Enums;
using OrderProcessingService.Application.Models;
using OrderProcessingService.Domain.Abstractions;
using OrderProcessingService.Domain.Entities;
using OrderProcessingService.Domain.Enums;
using OrderProcessingService.Domain.Exceptions;
using Serilog;

namespace OrderProcessingService.Application.Services;

public class OrderProcessingService : IOrderProcessingService
{
    private IOrderProcessingRepository _processingRepository;

    public OrderProcessingService(IOrderProcessingRepository processingRepository)
        {
            _processingRepository = processingRepository;
        }

    public async Task<ProcessingOrderModel> AssembleOrderAsync(Guid id, CancellationToken ct)
    {
        Log.Information("Запуск метода AssembleOrderAsync для заказа с id: {Id}", id);

        var existingProcessingOrder = await _processingRepository.GetByIdAsync(id, ct);
        ValidateProcessingOrder(existingProcessingOrder, id);

        await _processingRepository.ChangeProcessingOrderStatusToProcessing(existingProcessingOrder, ct);

        var existingProcessingOrderModel = MapToProcessingOrderModel(existingProcessingOrder);

        var jobId = BackgroundJob.Enqueue<IWorkerSimulator>(worker =>
            worker.ProcessOrderInWarehouseAsync(existingProcessingOrderModel));
        
        Log.Information("Задача добавлена в очередь фоновых работ. Job ID: {JobId}", jobId);
        
        //var result = await _processingRepository.GetByIdAsync(id, ct);
        
        Log.Information("Успешное завершение метода AssembleOrderAsync для заказа с id: {Id}", id);
        return existingProcessingOrderModel;
    }

    public async Task<List<DeliveryOrderModel>> TakeOrdersForDeliveryAsync(List<Guid> guids, CancellationToken ct)
    {
        Log.Information("Запуск метода GetById для заказа с id: {Id}", guids);
        var deliveryOrderModels = new List<DeliveryOrderModel>();
        foreach (var guid in guids)
        {
            var order = await _processingRepository.GetByIdAsync(guid, ct);
            // не нравится, что если какой то заказ не прошел проверку, то надо заново запускать
            // хотел бы пройтись по всем заказам и те которые находятся в неправильно статусе отбросить
            ValidateOrderForDeliver(order, guid);
            Log.Information("Заказ передается в службу доставки");
            await _processingRepository.PrepareOrderForDelivery(order,ct);
            Log.Information("Заказ успешно передан в службу доставки");

            var deliveryOrderModel = MapToDeliveryOrderModel(order);
            deliveryOrderModels.Add(deliveryOrderModel);
        }
        
        var jobId = BackgroundJob.Enqueue<IWorkerSimulator>(worker =>
            worker.TransferOrderToDelivery(deliveryOrderModels.Select(f=>f.OrderId).ToList()));
        
        Log.Information("Задача добавлена в очередь фоновых работ. Job ID: {JobId}", jobId);
        
        Log.Information("Успешное завершение метода TakeOrdersForDeliveryAsync");
        return deliveryOrderModels;
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

    private void ValidateOrderForDeliver(ProcessingOrder existingProcessingOrder, Guid id)
    {
        if (existingProcessingOrder == null)
        {
            throw new OrderProcessingDoesNotExistsException(id.ToString());
        }
        
        if (existingProcessingOrder.Status != ProcessingOrderStatus.Completed 
            && existingProcessingOrder.Stage != Stage.Assembly)
        {
            throw new DeliverOrderException(
                existingProcessingOrder.OrderId,
                existingProcessingOrder.Status,
                existingProcessingOrder.Stage);
        }
    }
    
    private ProcessingOrderModel MapToProcessingOrderModel(ProcessingOrder existingProcessingOrder)
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
    
    private DeliveryOrderModel MapToDeliveryOrderModel(ProcessingOrder existingProcessingOrder)
    {
        return new DeliveryOrderModel()
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
            }).ToList(),
            TrackingNumber = existingProcessingOrder.TrackingNumber
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