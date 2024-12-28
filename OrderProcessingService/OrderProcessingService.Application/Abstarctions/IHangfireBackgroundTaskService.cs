using OrderProcessingService.Application.Models;

namespace OrderProcessingService.Application.Abstarctions;

public interface IHangfireBackgroundTaskService
{
    public void ScheduleProductAssemblyTask(Guid orderId, List<ProcessingOrderItemModel> items, CancellationToken ct);
}