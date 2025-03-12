using Messaging.Kafka.Models;
using Messaging.Kafka.Producer;
using OrderProcessingService.Application.Abstractions;
using OrderProcessingService.Application.Enums;
using OrderProcessingService.Application.Models;
using OrderProcessingService.Domain.Abstractions;
using OrderProcessingService.Domain.Enums;
using Serilog;
using StageModel = OrderProcessingService.Application.Enums.StageModel;

namespace OrderProcessingService.Application.Services;

public class WorkerSimulator : IWorkerSimulator
{
    private readonly IOrderProcessingRepository _processingRepository;
    private readonly IKafkaProducer<NotificationKafkaModel> _notificationProducer;
    private readonly IBackgroundJobService _backgroundJobService;

    public WorkerSimulator(
        IOrderProcessingRepository processingRepository,
        IKafkaProducer<NotificationKafkaModel> notificationProducer,
        IBackgroundJobService backgroundJobService)
    {
        _processingRepository = processingRepository;
        _notificationProducer = notificationProducer;
        _backgroundJobService = backgroundJobService;
    }

    public async Task ProcessOrderInWarehouseAsync(ProcessingOrderModel processingOrderModel)
    {
        Log.Information("Постановка задачи на сборку товара с ID: {TaskId} в очередь", processingOrderModel.Id);
        _backgroundJobService.Enqueue(() => ProcessOrderInWarehouseInternalAsync(processingOrderModel));
    }

    private async Task ProcessOrderInWarehouseInternalAsync(ProcessingOrderModel processingOrderModel)
    {
        Log.Information("Начинаем сборку товара для задачи с ID: {TaskId}", processingOrderModel.Id);

        await Task.Delay(1000);
        foreach (var item in processingOrderModel.Items)
        {
            Log.Information("Пришел за товаром {ProductId}", item.ProductId);
            item.ProcessingOrderItemStatus = ProcessingOrderItemStatusModel.Ready;
            Log.Information("Статус позиции {ProductId} изменен на Ready", item.ProductId);
        }

        await Task.Delay(1000);
        Log.Information("Все позиции готовы. Меняем состояние сборки на Completed.");

        var existingProcessingOrder =
            await _processingRepository.GetByIdAsync(processingOrderModel.Id, CancellationToken.None);
        await _processingRepository.ChangeProcessingOrderStatusToCompleted(existingProcessingOrder,
            CancellationToken.None);

        await Task.Delay(1000);
        Log.Information("Процесс сборки завершен для заказа с ID: {OrderId}", processingOrderModel.Id);
    }

    public async Task TransferOrderToDelivery(List<Guid> orders)
    {
        Log.Information("Постановка задачи на доставку заказов в очередь");
        _backgroundJobService.Enqueue(() => TransferOrderToDeliveryInternal(orders));
    }

    private async Task TransferOrderToDeliveryInternal(List<Guid> orders)
    {
        Log.Information("Начинаю доставлять заказы");

        foreach (var order in orders)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            var deliveryAddress = GenerateRandomAddress();
            Log.Information("Везу заказ {OrderId} по адресу: {Address}", order,
                deliveryAddress);

            await Task.Delay(TimeSpan.FromSeconds(3));
            Log.Information("Заказ {OrderId} доставлен по адресу: {Address}", order,
                deliveryAddress);

            await _processingRepository.ChangeOrderStatusToDeliveredAsync(order, CancellationToken.None);
            Log.Information("Изменен статус заказа на {status}", ProcessingOrderStatus.Completed);

            var notificationKafkaModel = new NotificationKafkaModel()
            {
                OrderId = order,
                Stage = Messaging.Kafka.Models.StageModel.Completed,
                Code = GenerateCodeToDelivery()
            };
            await _notificationProducer.ProduceAsync(notificationKafkaModel, cancellationToken: default);
        }

        Log.Information("Все заказы успешно доставлены.");
    }

    private string GenerateCodeToDelivery()
    {
        var random = new Random();
        var code = random.Next(0, 10000);
        var formattedCode = code.ToString("D4");
        return formattedCode;
    }

    private string GenerateRandomAddress()
    {
        var cities = new[] { "Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург" };
        var streets = new[] { "Центральная", "Ленина", "Пушкина", "Гагарина" };
        return $"{cities[new Random().Next(cities.Length)]}, улица {streets[new Random().Next(streets.Length)]}";
    }
}