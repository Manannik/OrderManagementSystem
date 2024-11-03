using Order.Domain.Entities;

namespace Order.Domain.Abstractions;

public interface IOrderService
{
    Task<Guid> CreateAsync(List<ProductItem> productItems, CancellationToken ct);
}