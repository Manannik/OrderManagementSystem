using OrderProcessingService.Domain.Enums;

namespace OrderProcessingService.Domain.Exceptions;

public class DeliverOrderException : Exception
{
    public string Message { get; }
    public int StatusCode { get; }
    public Guid OrderId { get; }
    public ProcessingOrderStatus Status { get; }
    public Stage Stage { get; }

    public DeliverOrderException(
        Guid orderId,
        ProcessingOrderStatus status,
        Stage stage)
    {
        Message =
            $"заказ '{orderId}'находится в статусе '{status}' на этапе сборки '{stage}'.";
        StatusCode = 410;
        OrderId = orderId;
        Status = status;
        Stage = stage;
    }
}