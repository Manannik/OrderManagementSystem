using Hangfire;
using Messaging.Kafka.Models;
using Messaging.Kafka.Producer;
using Microsoft.AspNetCore.Mvc;
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
    private IKafkaProducer<NotificationKafkaModel> _notificationProducer;
    public OrderProcessingService(
        IOrderProcessingRepository processingRepository, 
        IKafkaProducer<NotificationKafkaModel> notificationProducer)
    {
        _processingRepository = processingRepository;
        _notificationProducer = notificationProducer;
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

    public async Task<List<DeliveryOrderModel>> TakeOrdersForDeliveryAsync([FromBody] List<Guid> guids,
        CancellationToken ct)
    {
        Log.Information("Запуск метода TakeOrdersForDeliveryAsync для заказов с id: {Ids}", guids);
        var deliveryOrderModels = new List<DeliveryOrderModel>();
        var validOrderIds = new List<Guid>();

        foreach (var guid in guids)
        {
            var order = await _processingRepository.GetByIdAsync(guid, ct);

            if (ValidateOrderForDeliver(order, guid))
            {
                Log.Information("Заказ {OrderId} проходит проверку и готовится к доставке", guid);
                await _processingRepository.PrepareOrderForDelivery(order, ct);
                Log.Information("Заказ {OrderId} успешно подготовлен для доставки, " +
                                "ваш трек-номер {TrackingNumber}", guid, order.TrackingNumber);
                
                var notificationKafkaModel = new NotificationKafkaModel()
                {
                    //попробовать добавить код вручения
                    OrderId = guid,
                    Value = $"заказ {guid} успешно подготовлен для доставки"
                };
                
                await _notificationProducer.ProduceAsync(notificationKafkaModel, cancellationToken: default);
                
                var deliveryOrderModel = MapToDeliveryOrderModel(order);
                deliveryOrderModels.Add(deliveryOrderModel);
                validOrderIds.Add(guid);

                var jobId = BackgroundJob.Enqueue<IWorkerSimulator>(worker =>
                    worker.TransferOrderToDelivery(validOrderIds));

                Log.Information("Фоновая задача добавлена в очередь. Job ID: {JobId}", jobId);
            }
        }

        if (!validOrderIds.Any())
        {
            throw new InvalidOperationException("Отсутствуют заказы для доставки");
        }

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

    private bool ValidateOrderForDeliver(ProcessingOrder existingProcessingOrder, Guid id)
    {
        if (existingProcessingOrder == null)
        {
            Log.Warning("Заказ с id {OrderId} не найден.", id);
            return false;
        }

        if (existingProcessingOrder.Status == ProcessingOrderStatus.Completed
            && existingProcessingOrder.Stage == Stage.Assembly)
        {
            Log.Warning(
                "Заказ с id {OrderId} не может быть передан в доставку. Текущий статус: {Status}, этап: {Stage}",
                existingProcessingOrder.OrderId,
                existingProcessingOrder.Status,
                existingProcessingOrder.Stage);
            return true;
        }

        return false;
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