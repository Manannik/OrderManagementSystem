using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Domain.Abstarctions
{
    public interface IOrderRepository
    {
        Task<Entities.Order> CreateAsync(List<ProductItem> productItems, CancellationToken ct);
        Task<Entities.Order?> GetByIdAsync(Guid id, CancellationToken ct);

        Task<Domain.Entities.Order?> UpdateStatusAsync(Domain.Entities.Order order,
            OrderStatus status, CancellationToken ct);
    }
}