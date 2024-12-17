using System.Text.Json;
using Confluent.Kafka;

namespace OrderProcessingService.Infrastructure.Kafka;

public class KafkaJsonDeserializer<TMessage> : IDeserializer<TMessage>
{
    public TMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull) return default;
        return JsonSerializer.Deserialize<TMessage>(data);
    }
}