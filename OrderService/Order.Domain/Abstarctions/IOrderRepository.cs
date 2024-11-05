using Order.Domain.Entities;

namespace Order.Domain.Abstarctions;

public interface IOrderRepository
{
    Task<Entities.Order> CreateAsync(List<ProductItem> productItems, CancellationToken ct);
}