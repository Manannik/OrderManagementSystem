namespace Order.Application.Models
{
    public class CreateOrderRequest
    {
        public List<ProductItemModel> ProductItemModels { get; set; }
    }
}