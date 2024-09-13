namespace Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Category> Categories { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedDateUtc { get; set; }
    public DateTime UpdatedDateUtc { get; set; }

    public Product()
    {
        Categories = new List<Category>();
    }
}