namespace Order.Domain.Entities
{
    public class ProductItem
    {
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public List<Order> Orders { get; set; }
        public ProductItem()
    {
        Orders = new List<Order>();
    }
    }
}