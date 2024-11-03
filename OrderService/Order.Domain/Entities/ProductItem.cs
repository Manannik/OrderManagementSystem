namespace Order.Domain.Entities;

public class ProductItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public List<Order> Orders { get; set; }
    public ProductItem()
    {
        Orders = new List<Order>();
    }
}