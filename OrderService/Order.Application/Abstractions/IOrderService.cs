using Order.Domain.Entities;

namespace Order.Application.Abstractions;

public interface IOrderService
{
    Task<Domain.Entities.Order> CreateAsync(List<ProductItem> productItems, CancellationToken ct);
}