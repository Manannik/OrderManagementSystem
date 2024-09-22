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

        var categoriesId = request.CategoriesModelDtos.Select(f => f.Id).ToList();
        var existingCategories = await categoryRepository.GetByIdAsync(categoriesId, ct);

        // комментарий для себя продебажить
        var intersectedCategoriesId = existingCategories
            .Select(f => f.Id)
            .Intersect(categoriesId)
            .ToList();

        if (intersectedCategoriesId.Count == 0)
        {
            throw new WrongCategoryException(intersectedCategoriesId);
        }

        var product = new Product()
        {
            //id создавать в БД?
            // Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Categories = existingCategories,
            Price = request.Price,
            Quantity = request.Quantity,
            CreatedDateUtc = DateTime.UtcNow,
        };

        await productRepository.CreateAsync(product, ct);
        
        // return product.Id;
    }
}