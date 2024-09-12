using Domain.Entities;

namespace Domain.Abstractions;

public interface IProductRepository
{
    public Task CreateAsync(Product product, CancellationToken ct);
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    public Task UpdateAsync(Product product, CancellationToken ct);
    public Task DeleteAsync(Product product, CancellationToken ct);
    public Task<bool> ExistAsync(string name, CancellationToken ct);
    public Task UpdateQuantityAsync(Product product, CancellationToken ct);
}