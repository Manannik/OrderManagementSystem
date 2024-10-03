using Application.BusinessLogic.Models;

namespace Application.Models;
public class UpdateProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<CategoryModelDto> Category { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}