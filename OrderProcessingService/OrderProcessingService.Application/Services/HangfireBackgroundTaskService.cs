using Hangfire;
using Microsoft.Extensions.Logging;
using OrderProcessingService.Application.Abstarctions;
using OrderProcessingService.Application.Enums;
using OrderProcessingService.Application.Models;

namespace OrderProcessingService.Application.Services;

public class HangfireBackgroundTaskService : IHangfireBackgroundTaskService
{
    private ILogger<OrderProcessingService> _logger;
    private IOrderProcessingService _orderProcessingService;

    public HangfireBackgroundTaskService(ILogger<OrderProcessingService> logger,
        IOrderProcessingService orderProcessingService)
    {
        _logger = logger;
        _orderProcessingService = orderProcessingService;
    }

    public void ScheduleProductAssemblyTask(Guid orderId, List<ProcessingOrderItemModel> items, CancellationToken ct)
    {
        BackgroundJob.Enqueue(() => StartProductAssembly(orderId, items, ct));
    }

    private async Task StartProductAssembly(Guid orderId, List<ProcessingOrderItemModel> items, CancellationToken ct)
    {
        _logger.LogInformation("Начинаем сборку товара для задачи с ID: {TaskId}", orderId);
        await Task.Delay(1000, ct);
        foreach (var item in items)
        {
            _logger.LogInformation("Пришел за товаром {ProductId}", item.ProductId);
            item.ProcessingOrderItemStatus = ProcessingOrderItemStatusModel.Ready;
            _logger.LogInformation("Статус позиции {ProductId} изменен на Ready", item.ProductId);
        }
        await Task.Delay(1000, ct);
        _logger.LogInformation("Все позиции готовы. Меняем состояние сборки на Completed.");
        await _orderProcessingService.UpdateProcessingOrderStatusToCompletedAsync(orderId, items, ct);
        
        await Task.Delay(1000, ct);
        _logger.LogInformation("Процесс сборки завершен для заказа с ID: {OrderId}", orderId);
    }
}