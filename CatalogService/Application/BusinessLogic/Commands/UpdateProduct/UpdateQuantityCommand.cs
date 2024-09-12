using Application.BusinessLogic.Models;
using Domain.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Commands.UpdateProduct;

public class UpdateQuantityCommand : IRequest<ProductModel>
{
    public Guid Id { get; set; }
    public int NewQuantity { get; set; }
    public UpdateQuantityCommand(Guid id,int newQuantity)
    {
        NewQuantity = newQuantity;
        Id = id;
    }
}

public class UpdateQuantityCommandHandler(IProductRepository productRepository) : IRequestHandler<UpdateQuantityCommand,ProductModel>
{
    public async Task<ProductModel> Handle(UpdateQuantityCommand request, CancellationToken ct)
    {
        var existingProduct = await productRepository.GetByIdAsync(request.Id, ct);
        if (existingProduct == null)
        {
            throw new ProductDoesNotExistException(request.Id);
        }

        existingProduct.Quantity = request.NewQuantity;
        existingProduct.UpdatedDateUtc = DateTime.UtcNow;
        
        await productRepository.UpdateQuantityAsync(existingProduct, ct);

        return new ProductModel()
        {
            Id = existingProduct.Id,
            Name = existingProduct.Name,
            Description = existingProduct.Description,
            Price = existingProduct.Price,
            Category = new CategoryModel()
            {
                Name = existingProduct.Category.Name
            },
            Quantity = existingProduct.Quantity,
            CreatedDateUtc = existingProduct.CreatedDateUtc,
            UpdatedDateUtc = existingProduct.UpdatedDateUtc
        };
    }
}