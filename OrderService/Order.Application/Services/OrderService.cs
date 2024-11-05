using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Order.Persistence;

namespace Order.Application.Services;

public class OrderService(OrderDbContext dbContext, ILogger<OrderService> _logger) : IOrderService
{
    public async Task<Domain.Entities.Order> CreateAsync(List<ProductItem> productItems, CancellationToken ct)
    {
        if (productItems == null || !productItems.Any())
        {
            throw new EmptyProductsException();
        }

        _logger.LogInformation("Запуск метода CreateAsync для списка продуктов: {productItems}",
            productItems.Select(f => f.ProductId));

        var existingProductItems = await dbContext.ProductItems
            .Where(f => productItems.Select(g => g.ProductId).Contains(f.ProductId))
            .ToListAsync(ct);

        var newProductItems = productItems
            .Where(f => existingProductItems.All(e => e.ProductId != f.ProductId))
            .ToList();

        if (newProductItems.Any())
        {
            _logger.LogInformation("В базе недостаточно продуктов, добавляем {newProductItems} с заданным количеством",
                newProductItems.Select(f => f.ProductId));
            await dbContext.ProductItems.AddRangeAsync(newProductItems, ct);
        }

        var newOrder = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderStatus = OrderStatus.Сreated,
            ProductItems = existingProductItems
        };
        
        existingProductItems.ForEach(f => f.Quantity += productItems.SingleOrDefault(g => g.ProductId == f.ProductId).Quantity);
        existingProductItems.ForEach(f => f.Orders.Add(newOrder));

        newOrder.CalculateCost(productItems);
        
        await dbContext.Orders.AddAsync(newOrder, ct);
        await dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("Успешное завершение CreateAsync для списка продуктов: {productItems}",
            productItems.Select(f => f.ProductId));
        return newOrder;
    }
}