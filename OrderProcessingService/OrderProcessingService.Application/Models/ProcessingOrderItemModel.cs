using OrderProcessingService.Application.Enums;

namespace OrderProcessingService.Application.Models;

public class ProcessingOrderItemModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public ProcessingOrderItemStatusModel ProcessingOrderItemStatus { get; set; }
}