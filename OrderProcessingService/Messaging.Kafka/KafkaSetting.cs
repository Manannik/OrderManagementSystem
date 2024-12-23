namespace Messaging.Kafka;

public class KafkaSetting
{
    public string BootstrapServers { get; set; }
    public string Topic { get; set; }
    public string GroupId { get; set; }
}