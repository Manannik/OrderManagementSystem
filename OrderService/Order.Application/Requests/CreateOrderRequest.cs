using Order.Application.Models;

namespace Order.Application.Requests
{
    public class CreateOrderRequest
    {
        public List<ProductItemModel> ProductItemModels { get; set; }
    }
}