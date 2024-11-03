using Microsoft.AspNetCore.Mvc;
using Order.Application.Models;
using Order.Domain.Abstractions;

namespace Order.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly ICatalogServiceClient _catalogServiceClient;

    public OrderController(ILogger<OrderController> logger,
        IOrderService orderService,
        ICatalogServiceClient catalogServiceClient
    )
    {
        _logger = logger;
        _orderService = orderService;
        _catalogServiceClient = catalogServiceClient;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        _logger.LogInformation("запуск метод Create, request: {@Request}", request);

        var products = await Task.WhenAll(
            request.ProductGuids.Select(f => _catalogServiceClient.ChangeProductQuantityAsync(f, ct))
        );
        
        // var products = new List<ProductItem>();
        //
        // foreach (var id in request.ProductGuids)
        // {
        //     var product = await _catalogServiceClient.ChangeProductQuantityAsync(id, ct);
        //     products.Add(product);
        // }

        await _orderService.CreateAsync(products.ToList(), ct);

        _logger.LogInformation("в результате работы метода Create, заказ успешно создан");
        return Ok();
    }
}