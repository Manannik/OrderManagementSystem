using Order.Domain.Entities;

namespace Order.Domain.Abstractions;

public interface IOrderService
{
    Task<Entities.Order> CreateAsync(List<ProductItem> productItems, CancellationToken ct);
}