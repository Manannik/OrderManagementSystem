using Order.Domain.Entities;

namespace Order.Application.Abstractions;

public interface ICatalogServiceClient
{
    Task<ProductItem> ChangeProductQuantityAsync(Guid id, int newQuantity, CancellationToken ct);
    Task<bool> ProductExistsAsync(Guid id, CancellationToken ct);
}