﻿using Application.BusinessLogic.Models;
using Application.Models;
using Domain.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Commands.UpdateProduct;

public class UpdateQuantityCommand : IRequest<ProductModelDto>
{
    public Guid Id { get; set; }
    public OrderedQuantity Request { get; set; }
}

public class UpdateQuantityCommandHandler(IProductRepository productRepository)
    : IRequestHandler<UpdateQuantityCommand, ProductModelDto>
{
    public async Task<ProductModelDto> Handle(UpdateQuantityCommand request, CancellationToken ct)
    {
        var existingProduct = await productRepository.GetByIdAsync(request.Id, ct);
        if (existingProduct == null)
        {
            throw new ProductDoesNotExistException(request.Id);
        }

        existingProduct.Quantity -= request.Request.Quantity;

        if (existingProduct.Quantity < 0)
        {
            throw new QuantityException();
        }

        existingProduct.UpdatedDateUtc = DateTime.UtcNow;

        await productRepository.UpdateAsync(existingProduct, ct);

        return new ProductModelDto()
        {
            Id = existingProduct.Id,
            Name = existingProduct.Name,
            Description = existingProduct.Description,
            Price = existingProduct.Price,
            CategoriesModelDtos = existingProduct.Categories.Select(f => new CategoryModelDto()
            {
                Id = f.Id
            }).ToList(),
            Quantity = existingProduct.Quantity,
            CreatedDateUtc = existingProduct.CreatedDateUtc,
            UpdatedDateUtc = existingProduct.UpdatedDateUtc
        };
    }
}