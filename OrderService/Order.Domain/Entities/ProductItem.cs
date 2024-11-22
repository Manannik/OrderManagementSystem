using System.Text.Json.Serialization;

namespace Order.Domain.Entities
{
    public class ProductItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

    }
}