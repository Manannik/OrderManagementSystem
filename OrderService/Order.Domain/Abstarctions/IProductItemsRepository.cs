using Order.Domain.Entities;

namespace Order.Domain.Abstarctions;

public interface IProductItemsRepository
{
    Task<List<ProductItem>> GetAllByIdsAsync(List<Guid> guids, CancellationToken ct);
    Task AddRangeAsync(List<ProductItem> productItems, CancellationToken ct);
    Task ChangeQuantityAsync(List<ProductItem> productItems, CancellationToken ct);
}