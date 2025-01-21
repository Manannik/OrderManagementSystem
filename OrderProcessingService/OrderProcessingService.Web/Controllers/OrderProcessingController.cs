using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingService.Application.Abstractions;

namespace OrderProcessingService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderProcessingController : ControllerBase
    {
        private readonly ILogger<OrderProcessingController> _logger;

        private readonly IOrderProcessingService _orderProcessingService;
        public OrderProcessingController(ILogger<OrderProcessingController> logger,
            IOrderProcessingService orderProcessingService)
        {
            _logger = logger;
            _orderProcessingService = orderProcessingService;
        }

        [HttpPost("ProcessOrder")]
        public async Task<IActionResult> ProcessOrder([FromBody]Guid processingOrderId, CancellationToken ct)
        {
            _logger.LogInformation("запуск метод ProcessOrder для товара с Id: {@Request}", processingOrderId);
            
            var existingOrderProcessingModel =
                await _orderProcessingService.ProcessOrderByIdAsync(processingOrderId, ct);
            
            _logger.LogInformation("Заказ с id = {@Request} готов к отправке:", processingOrderId);
            return Ok(existingOrderProcessingModel);
        }

        [HttpPost("DeliverOrder")]
        public async Task<IActionResult> DeliverOrder([FromRoute]Guid processingOrderId, CancellationToken ct)
        {
            return Ok();
        }
    }
}