using OrderProcessingService.Domain.Enums;

namespace OrderProcessingService.Application.Abstarctions;

public interface IProcessingOrderStatusHandler
{
    void ValidateTransition(ProcessingOrderStatus currentStatus, ProcessingOrderStatus newStatus);
}