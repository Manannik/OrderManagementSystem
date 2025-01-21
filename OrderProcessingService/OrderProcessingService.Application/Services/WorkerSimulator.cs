using Microsoft.Extensions.Logging;
using OrderProcessingService.Application.Abstractions;
using OrderProcessingService.Application.Enums;
using OrderProcessingService.Application.Models;
using OrderProcessingService.Domain.Abstractions;
using OrderProcessingService.Domain.Entities;

namespace OrderProcessingService.Application.Services;

public class WorkerSimulator : IWorkerSimulator
{
    private ILogger<OrderProcessingService> _logger;
    private IOrderProcessingRepository _processingRepository;

    public WorkerSimulator(ILogger<OrderProcessingService> logger, IOrderProcessingRepository processingRepository)
    {
        _logger = logger;
        _processingRepository = processingRepository;
    }

    public async Task SimulateAsync(ProcessingOrderModel processingOrderModel, ProcessingOrder processingOrder)
    {
        _logger.LogInformation("Начинаем сборку товара для задачи с ID: {TaskId}", processingOrderModel.Id);
        await Task.Delay(1000);
        foreach (var item in processingOrderModel.Items)
        {
            _logger.LogInformation("Пришел за товаром {ProductId}", item.ProductId);
            item.ProcessingOrderItemStatus = ProcessingOrderItemStatusModel.Ready;
            _logger.LogInformation("Статус позиции {ProductId} изменен на Ready", item.ProductId);
        }
        await Task.Delay(1000);
        _logger.LogInformation("Все позиции готовы. Меняем состояние сборки на Completed.");
        
        var existingProcessingOrder = await _processingRepository.GetByIdAsync(processingOrder.Id, CancellationToken.None);
        await _processingRepository.ChangeProcessingOrderStatusToCompleted(existingProcessingOrder, CancellationToken.None);
        
        await Task.Delay(1000);
        _logger.LogInformation("Процесс сборки завершен для заказа с ID: {OrderId}", processingOrder.Id);
    }
}