using Application.BusinessLogic.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Commands.CreateProduct;

public class CreateProductCommand : IRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<CategoryModelDto> CategoriesModelDtos { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
}

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository) : IRequestHandler<CreateProductCommand>
{
    public async Task Handle(CreateProductCommand request, CancellationToken ct)
    {
        var isProductExist = await productRepository.ExistAsync(request.Name, ct);

        if (isProductExist)
        {
            throw new ProductAlreadyExistException(request.Name);
        }

        // стоит ли проверять категорию продукта ?
        var categoriesNames = request.CategoriesModelDtos.Select(f => f.Name).ToList();
        var existingCategories = await categoryRepository.GetByNamesAsync(categoriesNames, ct);

        var intersectedCategoriesNames = existingCategories
            .Select(f => f.Name)
            .Intersect(categoriesNames)
            .ToList();

        if (intersectedCategoriesNames != null)
        {
            throw new WrongCategoryException(intersectedCategoriesNames);
        }

        var product = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Categories = existingCategories,
            Price = request.Price,
            Quantity = request.Quantity,
            CreatedDateUtc = DateTime.UtcNow,
        };

        await productRepository.CreateAsync(product, ct);
    }
}