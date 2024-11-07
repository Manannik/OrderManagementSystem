using Order.Domain.Abstarctions;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Persistence.Repositories;

public class OrderRepository(OrderDbContext dbContext) : IOrderRepository
{
    public async Task<Domain.Entities.Order> CreateAsync(List<ProductItem> productItems, CancellationToken ct)
    {
        var newOrder = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderStatus = OrderStatus.Сreated,
            ProductItems = productItems
        };
        newOrder.CalculateCost(productItems);
        await dbContext.Orders.AddAsync(newOrder, ct);
        return newOrder;
    }
}