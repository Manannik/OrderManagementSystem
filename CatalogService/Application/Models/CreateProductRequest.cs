using Application.BusinessLogic.Models;

namespace Application.Models;

public class CreateProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<CategoryModelDto> CategoryModelDtos { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}