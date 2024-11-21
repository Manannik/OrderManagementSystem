using Microsoft.Extensions.Options;
using Order.Infrastructure.Kafka;

namespace WebApplication.Kafka;

// public class KafkaConsumerService : BackgroundService
// {
//     //private readonly KafkaConsumer<OrderModel> consumer;
//
//     public KafkaConsumerService(IOptions<KafkaSettings> kafkaSettings)
//     {
//         //consumer = new KafkaConsumer<OrderModel>(kafkaSettings);
//     }
//
//     protected override Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         throw new NotImplementedException();
//     }
// }
