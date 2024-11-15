using Order.Application.Models;

namespace Order.Application.Abstractions
{
    public interface IOrderService
    {
        Task<OrderModel> CreateAsync(CreateOrderRequest request, CancellationToken ct);
        Task<OrderModel> UpdateAsync(ChangeOrderStatusRequest request, CancellationToken ct);
    }
}