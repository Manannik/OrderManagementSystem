using Order.Domain.Enums;

namespace Order.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public decimal Cost { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public List<ProductItem> ProductItems { get; set; }
    public Order()
    {
        ProductItems = new List<ProductItem>();
    }
    
    public void CalculateCost()
    {
        Cost = ProductItems?.Sum(item => item.Price * item.Quantity) ?? 0;
    }
}