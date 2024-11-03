using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Domain.Abstractions;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Exceptions;

namespace Order.Persistence.Services;

public class OrderService(OrderDbContext dbContext) : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    public async Task<Guid> CreateAsync(List<ProductItem> productItems, CancellationToken ct)
    {
        if (productItems == null || !productItems.Any())
        {
            throw new EmptyProductsException();
        }
        
        _logger.LogInformation("Запуск метода CreateAsync для списка продуктов: {productItems}", productItems.Select(f=>f.Id));
        
        var newOrder = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            OrderStatus = OrderStatus.Сreated,
            ProductItems = productItems
        };

        var existingProductItems = await dbContext.ProductItems
            .Where(f => productItems.Any(g => g.Id == f.Id))
            .ToListAsync(ct);

        var newProductItems = productItems
            .Where(f => existingProductItems.All(e => e.Id != f.Id))
            .ToList();
        
        if (newProductItems.Any())
        {
            _logger.LogInformation("В базе недостаточно продуктов, добавляем {newProductItems}",newProductItems.Select(f=>f.Id));
            await dbContext.ProductItems.AddRangeAsync(newProductItems, ct);
            existingProductItems.AddRange(newProductItems);
        }
        
        existingProductItems.ForEach(f => f.Orders.Add(newOrder));
        
        newOrder.CalculateCost();
        await dbContext.Orders.AddAsync(newOrder, ct);
        await dbContext.SaveChangesAsync(ct);
        _logger.LogInformation("Успешное завершение CreateAsync для списка продуктов: {productItems}", productItems.Select(f=>f.Id));
        return newOrder.Id;
    }
}