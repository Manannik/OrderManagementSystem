using Order.Domain.Entities;

namespace Order.Domain.Abstractions
{
    public interface IProductItemsRepository
    {
        Task AddRangeAsync(List<ProductItem> productItems, Guid id, CancellationToken ct);
    }
}