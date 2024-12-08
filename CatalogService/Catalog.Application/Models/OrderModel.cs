namespace Application.Models
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public List<ProductModel> ProductModels { get; set; }
    }
}