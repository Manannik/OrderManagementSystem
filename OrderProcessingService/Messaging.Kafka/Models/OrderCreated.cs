namespace Messaging.Kafka.Models;

public class OrderCreated
{
    public Guid Id { get; set; }
    public decimal Cost { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ItemModel> Items { get; set; }
}