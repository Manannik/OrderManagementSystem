using Application.Models;
using Confluent.Kafka;
using Newtonsoft.Json;
using OrderManagementSystem.Infrastructure;

namespace WebApplication.Kafka;

public class KafkaConsumer(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => { _ = ConsumeAsync("order-topic", stoppingToken); }, stoppingToken);
    }

    public async Task ConsumeAsync(string topic, CancellationToken ct)
    {
        var config = new ConsumerConfig
        {
            GroupId = "order-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(topic);

        while (!ct.IsCancellationRequested)
        {
            var consumerResult = consumer.Consume(ct);

            var order = JsonConvert.DeserializeObject<OrderModel>(consumerResult.Message.Value);
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

            var products = dbContext.Products
                .Where(f => order.ProductModels.Any(g => g.Id == f.Id));
            
            foreach (var product in products)
            {
                product.Quantity = products.SingleOrDefault(f => f.Id == product.Id).Quantity;
            }

            await dbContext.SaveChangesAsync();
        }

        consumer.Close();
    }
}