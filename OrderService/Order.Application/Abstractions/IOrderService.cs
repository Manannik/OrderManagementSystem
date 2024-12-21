using Order.Application.Models;
using Order.Application.Requests;

namespace Order.Application.Abstractions
{
    public interface IOrderService
    {
        Task<OrderModelResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct);
        Task<OrderModelResponse> UpdateAsync(ChangeOrderStatusRequest request, CancellationToken ct);
    }
}