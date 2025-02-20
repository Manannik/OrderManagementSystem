namespace Messaging.Kafka.Models;

public class NotificationKafkaModel
{
    public Guid OrderId { get; set; }
    public string Value { get; set; }
}