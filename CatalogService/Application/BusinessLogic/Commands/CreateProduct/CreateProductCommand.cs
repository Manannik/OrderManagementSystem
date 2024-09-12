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
    public CategoryModel Category { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public CreateProductCommand(string name, string description, CategoryModel category, decimal price, int quantity)
    {
        Name = name;
        Description = description;
        Category = category;
        Price = price;
        Quantity = quantity;
    }
}

public class CreateProductCommandHandler(IProductRepository productRepository,
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
        var existingCategory = await categoryRepository.GetByNameAsync(request.Category.Name, ct);
        if (existingCategory == null)
        {
            throw new WrongCategoryException(request.Category.Name);
        }

        var product = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Category = existingCategory,
            Price = request.Price,
            Quantity = request.Quantity,
            CreatedDateUtc = DateTime.UtcNow,
        };

        await productRepository.CreateAsync(product, ct);
    }
}
