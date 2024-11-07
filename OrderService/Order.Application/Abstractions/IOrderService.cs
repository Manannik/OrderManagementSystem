using Order.Application.Models;

namespace Order.Application.Abstractions;

public interface IOrderService
{
    Task<Domain.Entities.Order> CreateAsync(CreateOrderRequest request, CancellationToken ct);
}