using OrderProcessingService.Domain.Enums;

namespace OrderProcessingService.Domain.Entities;

public class Item
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
    
    public Guid ProcessingOrderId { get; set; }
    public ProcessingOrder ProcessingOrder { get; set; }
}