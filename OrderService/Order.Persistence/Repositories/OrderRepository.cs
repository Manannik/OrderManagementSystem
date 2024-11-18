using Microsoft.EntityFrameworkCore;
using Order.Domain.Abstractions;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Persistence.Repositories
{
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
        await dbContext.SaveChangesAsync(ct);
        return newOrder;
    }

        public async Task<Domain.Entities.Order?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await dbContext.Orders.FirstOrDefaultAsync(f => f.Id == id,ct);
        }

        public async Task<Domain.Entities.Order?> UpdateStatusAsync(Domain.Entities.Order order, 
            OrderStatus status, CancellationToken ct)
        {
            order.OrderStatus = status;
            await dbContext.SaveChangesAsync(ct);

            return order;
        }
    }
}