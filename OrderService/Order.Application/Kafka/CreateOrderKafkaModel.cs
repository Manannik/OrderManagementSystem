namespace Order.Application.Models;

public class CreateOrderKafkaModel
{
    public Guid Id { get; set; }
    public decimal Cost { get; set; }
    public string OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}