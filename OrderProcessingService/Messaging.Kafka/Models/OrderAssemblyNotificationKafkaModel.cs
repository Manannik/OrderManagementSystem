namespace Messaging.Kafka.Models;

public class OrderAssemblyNotificationKafkaModel
{
    public Guid OrderId { get; set; }
    public StageModel Stage { get; set; }
    public string Value { get; set; }
    public Guid TrackingNumber { get; set; }
}