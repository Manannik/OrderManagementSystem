using System.Data;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Order.Application.Abstractions;
using Order.Application.Models;

namespace Order.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly IKafkaProducer _producer;
        public OrderController(ILogger<OrderController> logger,
            IOrderService orderService,
            ICatalogServiceClient catalogServiceClient, IKafkaProducer producer)
        {
            _logger = logger;
            _orderService = orderService;
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
        {
            _logger.LogInformation("запуск метод Create, request: {@Request}", request);

            var order = await _orderService.CreateAsync(request, ct);
        
            _logger.LogInformation("в результате работы метода Create, заказ успешно создан");
            return Ok(order);
        
        }

        [HttpPut("ChangeStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] ChangeOrderStatusRequest request, CancellationToken ct)
        {
            _logger.LogInformation("запуск метод UpdateStatus, request: {@Request}", request);
            
            var updatedOrder = await _orderService.UpdateAsync(request, ct);
            
            _logger.LogInformation("в результате работы метода UpdateStatus, статус заказа успешно изменен");
            return Ok(updatedOrder);
        }
    }
}