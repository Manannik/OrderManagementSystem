using Microsoft.AspNetCore.Mvc;
using OrderProcessingService.Application.Abstractions;
using Serilog;

namespace OrderProcessingService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderProcessingController : ControllerBase
    {
        private readonly IOrderProcessingService _orderProcessingService;
        public OrderProcessingController(IOrderProcessingService orderProcessingService)
        {
            _orderProcessingService = orderProcessingService;
        }

        [HttpPost("AssembleOnWarehouse")]
        public async Task<IActionResult> AssembleOnWarehouse([FromBody]Guid processingOrderId, CancellationToken ct)
        {
            Log.Information("запуск метода AssembleOnWarehouse для товара с Id: {@Request}", processingOrderId);
            
            var existingOrderProcessingModel =
                await _orderProcessingService.AssembleOrderAsync(processingOrderId, ct);
            
            Log.Information("Успешное завершение метода AssembleOnWarehouse. " +
                            "Заказ с id = {@Request} готов к отправке:", processingOrderId);
            return Ok(existingOrderProcessingModel);
        }

        [HttpPost("DeliverOrder")]
        public async Task<IActionResult> DeliverOrder([FromRoute]List<Guid> guids, CancellationToken ct)
        {
            Log.Information("Запуск метода DeliverOrder для товара с Id: {@Request}", guids);
            
            var existingOrderProcessingModel =
                await _orderProcessingService.TakeOrdersForDeliveryAsync(guids, ct);
            
            Log.Information("Успешное завершение метода DeliverOrder для товара с Id: {@Request}", guids);
            return Ok(existingOrderProcessingModel);
        }
    }
}