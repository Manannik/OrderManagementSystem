using Application.BusinessLogic.Models;
using Domain.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Queries
{
    public class GetProductQuery : IRequest<ProductModelDto>
    {
        public Guid ProductId { get; set; }
    }

    public class GetProductQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductQuery, ProductModelDto>
    {
        public async Task<ProductModelDto> Handle(GetProductQuery request, CancellationToken ct)
        {
            var existingProduct = await productRepository.GetByIdAsync(request.ProductId, ct);

            if (existingProduct == null)
            {
                throw new ProductDoesNotExistException(request.ProductId);
            }

            var productModel = new ProductModelDto()
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                CategoriesModelDtos = existingProduct.Categories.Select(f=>new CategoryModelDto()
                {
                    Id = f.Id,
                }).ToList(),
                Price = existingProduct.Price,
                Quantity = existingProduct.Quantity,
                CreatedDateUtc = existingProduct.CreatedDateUtc,
                UpdatedDateUtc = existingProduct?.UpdatedDateUtc
            };

            return productModel;
        }
    }
}
