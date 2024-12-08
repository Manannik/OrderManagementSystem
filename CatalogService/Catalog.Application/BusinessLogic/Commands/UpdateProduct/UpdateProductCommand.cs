using Application.BusinessLogic.Models;
using Application.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<ProductModelDto>
    {
        public Guid Id { get; set; }
        public UpdateProductRequest Request { get; set; }
    }

    public class UpdateProductCommandHandle(IProductRepository productRepository, 
        ICategoryRepository categoryRepository)
        : IRequestHandler<UpdateProductCommand, ProductModelDto>
    {
        public async Task<ProductModelDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var existingProduct = await productRepository.GetByIdAsync(request.Id, ct);
        
        if (existingProduct == null)
        {
            throw new ProductDoesNotExistException(request.Id);
        }

        var categoriesRequestIds = request.Request.CategoryModelDtos
            .Select(f => f.Id)
            .ToList();

        var existingCategories = await categoryRepository
            .GetByIdAsync(categoriesRequestIds, ct);
        
        var exceptedCategoriesIds = categoriesRequestIds
            .Except(existingCategories.Select(f => f.Id)).ToList();
        
        if (exceptedCategoriesIds.Count != 0)
        {
            throw new WrongCategoryException(exceptedCategoriesIds);
        }
        
        existingProduct.Name = request.Request.Name;
        existingProduct.Description = request.Request.Description;
        existingProduct.Price = request.Request.Price;
        existingProduct.Quantity = request.Request.Quantity;
        existingProduct.UpdatedDateUtc = DateTime.UtcNow;
        existingProduct.Categories = existingCategories;
        
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
}

