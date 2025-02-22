namespace Messaging.Kafka.Models;

public class DeliveryNotificationKafkaModel
{
    public Guid OrderId { get; set; }
    public string Value { get; set; }
}