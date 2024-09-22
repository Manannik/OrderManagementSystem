using Application.BusinessLogic.Models;
using Application.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<ProductModelDto>
{
    public Guid Id { get; set; }
    public UpdateProductRequest Request { get; set; }
}

public class UpdateProductCommandHandle(IProductRepository productRepository) : IRequestHandler<UpdateProductCommand, ProductModelDto>
{
    public async Task<ProductModelDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var existingProduct = await productRepository.GetByIdAsync(request.Id, ct);
        
        if (existingProduct == null)
        {
            throw new ProductDoesNotExistException(request.Id);
        }

        existingProduct.Name = request.Request.Name;
        existingProduct.Description = request.Request.Description;
        existingProduct.Price = request.Request.Price;
        existingProduct.Quantity = request.Request.Quantity;
        existingProduct.UpdatedDateUtc = DateTime.UtcNow;

        await productRepository.UpdateAsync(existingProduct, ct);

        var result = new ProductModelDto()
        {
            Id = existingProduct.Id,
            Name = existingProduct.Name,
            Description = existingProduct.Description,
            CategoriesModelDtos = existingProduct.Categories.Select(f=>new CategoryModelDto()
            {
                Id = f.Id
            }).ToList(),
            Price = existingProduct.Price,
            Quantity = existingProduct.Quantity,
            CreatedDateUtc = existingProduct.CreatedDateUtc,
            UpdatedDateUtc = existingProduct.UpdatedDateUtc,
        };

        return result;
    }
}

