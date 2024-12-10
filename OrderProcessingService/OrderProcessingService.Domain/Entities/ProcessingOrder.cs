using OrderProcessingService.Domain.Enums;

namespace OrderProcessingService.Domain.Entities;

public class ProcessingOrder
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public List<Item> Items { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public Stage Stage { get; set; }
    public ProcessingOrderStatus Status { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid TrackingNumber { get; set; }
    
}