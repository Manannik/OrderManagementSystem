using Confluent.Kafka;
using Messaging.Kafka.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Order.Application.Abstractions;
using Order.Application.Models;

namespace Order.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly ICatalogServiceClient _catalogServiceClient;
    private readonly IKafkaProducer _producer;
    public OrderController(ILogger<OrderController> logger,
        IOrderService orderService,
        ICatalogServiceClient catalogServiceClient, IKafkaProducer producer)
    {
        _logger = logger;
        _orderService = orderService;
        _catalogServiceClient = catalogServiceClient;
        _producer = producer;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        _logger.LogInformation("запуск метод Create, request: {@Request}", request);

        var order = await _orderService.CreateAsync(request, ct);
        
        await _producer.ProduceAsync("order-topic", new Message<string, string>()
        {
            Key = order.Id.ToString(),
            Value = JsonConvert.SerializeObject(order)
        });
        
        _logger.LogInformation("в результате работы метода Create, заказ успешно создан");
        return Ok();
        
    }
}