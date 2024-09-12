using Application.BusinessLogic.Models;
using Domain.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Queries
{
    public class GetProductQuery : IRequest<ProductModel>
    {
        public Guid ProductId { get; set; }
        public GetProductQuery(Guid productId)
        {
            ProductId = productId;
        }
    }

    public class GetProductQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductQuery, ProductModel>
    {
        public async Task<ProductModel> Handle(GetProductQuery request, CancellationToken ct)
        {
            var existingProduct = await productRepository.GetByIdAsync(request.ProductId, ct);

            if (existingProduct == null)
            {
                throw new ProductDoesNotExistException(request.ProductId);
            }

            var productModel = new ProductModel()
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                Category = new CategoryModel()
                {
                    Name = existingProduct.Category.Name,
                },
                Price = existingProduct.Price,
                Quantity = existingProduct.Quantity,
                CreatedDateUtc = existingProduct.CreatedDateUtc,
                UpdatedDateUtc = existingProduct?.UpdatedDateUtc
            };

            return productModel;
        }
    }
}
