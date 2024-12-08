namespace OrderProcessingService.Application.Models.Kafka;

public class ItemModel
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public OrderStatusModel Status { get; set; }
}