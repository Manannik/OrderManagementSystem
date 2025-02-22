namespace Messaging.Kafka.Models;

public class NotificationKafkaModel
{
    public Guid OrderId { get; set; }
    public StageModel Stage { get; set; }
    public string Code { get; set; }
    public Guid TrackingNumber { get; set; }
}