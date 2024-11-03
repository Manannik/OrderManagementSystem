namespace Order.Application.Models;

public class CreateOrderRequest
{
    public List<Guid> ProductGuids { get; set; }
}