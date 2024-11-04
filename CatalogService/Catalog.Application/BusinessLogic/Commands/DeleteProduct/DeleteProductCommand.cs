using Domain.Abstractions;
using Domain.Exceptions;
using MediatR;

namespace Application.BusinessLogic.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteProductCommandHandle(IProductRepository productRepository) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var existingProduct = await productRepository.GetByIdAsync(request.Id, ct);
        
        if (existingProduct == null)
        {
            throw new ProductDoesNotExistException(request.Id);
        }
        
        await productRepository.DeleteAsync(existingProduct,ct);
    }
}