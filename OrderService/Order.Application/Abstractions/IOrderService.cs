using Order.Application.Models;
using Order.Domain.Entities;

namespace Order.Application.Abstractions;

public interface IOrderService
{
    Task<Domain.Entities.Order> CreateAsync(CreateOrderRequest request, CancellationToken ct);
}