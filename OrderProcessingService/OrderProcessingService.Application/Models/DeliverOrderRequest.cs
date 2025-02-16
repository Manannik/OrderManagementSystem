namespace OrderProcessingService.Application.Models;

public class DeliverOrderRequest
{
    public List<Guid> Guids { get; set; }
}