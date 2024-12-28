using OrderProcessingService.Application.Abstarctions;
using OrderProcessingService.Application.Extensions;
using OrderProcessingService.Domain.Enums;
using OrderProcessingService.Domain.Exceptions;
using OrderProcessingService.Domain.Extensions;

namespace OrderProcessingService.Application.Services;

public class ProcessingOrderStatusHandler : IProcessingOrderStatusHandler
{
    public void ValidateTransition(ProcessingOrderStatus currentStatus, ProcessingOrderStatus newStatus)
    {
        switch (currentStatus)
        {
            case ProcessingOrderStatus.New:
                if (newStatus != ProcessingOrderStatus.Processing)
                {
                    throw new InvalidOrderStatusTransitionException($"Нельзя перейти из статуса {currentStatus.GetDisplayName()} в статус {newStatus.GetDisplayName()}.");
                }
                break;

            case ProcessingOrderStatus.Processing:
                if (newStatus != ProcessingOrderStatus.Completed)
                {
                    throw new InvalidOrderStatusTransitionException($"Нельзя перейти из статуса {currentStatus.GetDisplayName()} в статус {newStatus.GetDisplayName()}.");
                }
                break;

            case ProcessingOrderStatus.Completed:
                throw new InvalidOrderStatusTransitionException("Заказ уже выполнен и не может быть изменен.");
        }
    }
}
