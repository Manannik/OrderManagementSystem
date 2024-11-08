namespace Order.Application.Models;

public class ProductItemModel
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}