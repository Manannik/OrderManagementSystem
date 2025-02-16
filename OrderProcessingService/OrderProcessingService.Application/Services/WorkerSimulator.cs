using OrderProcessingService.Application.Abstractions;
using OrderProcessingService.Application.Enums;
using OrderProcessingService.Application.Models;
using OrderProcessingService.Domain.Abstractions;
using Serilog;

namespace OrderProcessingService.Application.Services;

public class WorkerSimulator : IWorkerSimulator
{
    private IOrderProcessingRepository _processingRepository;

    public WorkerSimulator(IOrderProcessingRepository processingRepository)
    {
        _processingRepository = processingRepository;
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
            await Task.Delay(TimeSpan.FromSeconds(30));
            var deliveryAddress = GenerateRandomAddress();
            Log.Information("Везу заказ {OrderId} по адресу: {Address}", order, 
                deliveryAddress);

            await Task.Delay(TimeSpan.FromSeconds(30));
            Log.Information("Заказ {OrderId} доставлен по адресу: {Address}", order, 
                deliveryAddress);

            await _processingRepository.ChangeOrderStatusToDeliveredAsync(order, CancellationToken.None);
        }

        Log.Information("Все заказы успешно доставлены.");
    }

    private string GenerateRandomAddress()
    {
        var cities = new[] { "Москва", "Санкт-Петербург", "Новосибирск", "Екатеринбург" };
        var streets = new[] { "Центральная", "Ленина", "Пушкина", "Гагарина" };
        return $"{cities[new Random().Next(cities.Length)]}, улица {streets[new Random().Next(streets.Length)]}";
    }
}