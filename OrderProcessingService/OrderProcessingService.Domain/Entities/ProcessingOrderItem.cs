using OrderProcessingService.Domain.Enums;

namespace OrderProcessingService.Domain.Entities;

public class ProcessingOrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public ProcessingOrderItemStatus ProcessingOrderItemStatus { get; set; }
    
    public Guid ProcessingOrderId { get; set; }
    public ProcessingOrder ProcessingOrder { get; set; }
}