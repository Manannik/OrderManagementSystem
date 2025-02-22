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
    private IOrderProcessingRepository _processingRepository;
    private IKafkaProducer<NotificationKafkaModel> _notificationProducer;

    public WorkerSimulator(
        IOrderProcessingRepository processingRepository,
        IKafkaProducer<NotificationKafkaModel> notificationProducer)
    {
        _processingRepository = processingRepository;
        _notificationProducer = notificationProducer;
    }

    public async Task ProcessOrderInWarehouseAsync(ProcessingOrderModel processingOrderModel)
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
            Log.Information("изменен статус заказа на {status}", ProcessingOrderStatus.Completed);
            
            var notificationKafkaModel = new NotificationKafkaModel()
            {
                //попробовать добавить код вручения
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
        Random random = new Random();
        int code = random.Next(0, 10000);
        string formattedCode = code.ToString("D4");
        return formattedCode;
    }

    private string GenerateRandomAddress()
    {
        var cities = new[] { "Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург" };
        var streets = new[] { "Центральная", "Ленина", "Пушкина", "Гагарина" };
        return $"{cities[new Random().Next(cities.Length)]}, улица {streets[new Random().Next(streets.Length)]}";
    }
}