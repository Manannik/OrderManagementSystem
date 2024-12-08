using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Abstractions;
using Order.Application.Models;
using Order.Application.Requests;

namespace Order.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly IValidator<CreateOrderRequest> _createOrderRequestValidator;
        private readonly IValidator<ChangeOrderStatusRequest> _changeOrderStatusRequestValidator;
        public OrderController(ILogger<OrderController> logger,
            IOrderService orderService, 
            IValidator<CreateOrderRequest> createOrderRequestValidator, 
            IValidator<ChangeOrderStatusRequest> changeOrderStatusRequestValidator)
        {
            _logger = logger;
            _orderService = orderService;
            _createOrderRequestValidator = createOrderRequestValidator;
            _changeOrderStatusRequestValidator = changeOrderStatusRequestValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
        {
            _logger.LogInformation("запуск метод Create, request: {@Request}", request);
            var validationResult = await _createOrderRequestValidator.ValidateAsync(request,ct);
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("в результате работы метода Create, заказ НЕ был создан");
                return BadRequest(validationResult.Errors);
            }
            
            var order = await _orderService.CreateAsync(request, ct);

            _logger.LogInformation("в результате работы метода Create, заказ успешно создан");
            return Ok(order);
        }

        [HttpPut("ChangeStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] ChangeOrderStatusRequest request, CancellationToken ct)
        {
            _logger.LogInformation("запуск метод UpdateStatus, request: {@Request}", request);

            var validationResult = await _changeOrderStatusRequestValidator.ValidateAsync(request,ct);
            
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("в результате работы метода UpdateStatus, заказ НЕ был изменен");
                return BadRequest(validationResult.Errors);
            }
            
            var updatedOrder = await _orderService.UpdateAsync(request, ct);

            _logger.LogInformation("в результате работы метода UpdateStatus, статус заказа успешно изменен");
            return Ok(updatedOrder);
        }
    }
}