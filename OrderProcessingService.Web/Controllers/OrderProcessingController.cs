using Microsoft.AspNetCore.Mvc;

namespace OrderProcessingService.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderProcessingController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    private readonly ILogger<OrderProcessingController> _logger;

    public OrderProcessingController(ILogger<OrderProcessingController> logger)
    {
        _logger = logger;
    }
}