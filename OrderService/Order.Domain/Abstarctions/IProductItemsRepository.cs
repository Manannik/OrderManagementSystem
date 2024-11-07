using Order.Domain.Entities;

namespace Order.Domain.Abstarctions;

public interface IProductItemsRepository
{
    Task AddRangeAsync(List<ProductItem> productItems, CancellationToken ct);
}