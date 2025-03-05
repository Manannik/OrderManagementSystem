namespace Messaging.Kafka.Models;

public class NotificationKafkaModel
{
    public Guid OrderId { get; set; }
    public StageModel Stage { get; set; }
}