using OrderProcessingService.Application.Enums;

namespace OrderProcessingService.Application.Models;

public class DeliveryOrderModel
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public List<ProcessingOrderItemModel> Items { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public StageModel Stage { get; set; }
    public ProcessingOrderStatusModel Status { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid TrackingNumber { get; set; }
}