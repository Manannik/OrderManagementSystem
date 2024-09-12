using Application.BusinessLogic.Models;
using Application.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Commands.CreateProduct;

public class CreateProductCommand_v2 : IRequest
{
    public CreateProductRequest Request { get; set; }
    
    public CreateProductCommand_v2(CreateProductRequest request)
    {
        Request = request;
    }
}

public class CreateProductCommand_v2Handler(IProductRepository productRepository, 
    ICategoryRepository categoryRepository) : IRequestHandler<CreateProductCommand>
{
    public async Task Handle(CreateProductCommand request, CancellationToken ct)
    {
        var isProductExist = await productRepository.ExistAsync(request.Name, ct);
        if (isProductExist)
        {
            throw new ProductAlreadyExistException(request.Name);
        }
        var existingCategory = await categoryRepository.GetByNameAsync(request.Category.Name, ct);
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
