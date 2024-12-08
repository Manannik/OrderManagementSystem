namespace Order.Application.Models;

public class UpdatedOrderKafkaModel
{
    public Guid Id { get; set; }
    public decimal Cost { get; set; }
    public string OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}