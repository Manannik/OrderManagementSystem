using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Application.Models;
using Order.Domain.Abstarctions;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Exceptions;
using Order.Persistence;
using Order.Persistence.Repositories;

namespace Order.Application.Services;

public class OrderService(
    ILogger<OrderService> logger,
    IOrderRepository orderRepository,
    IProductItemsRepository productItemsRepository,
    ICatalogServiceClient catalogServiceClient) : IOrderService
{
    public async Task<Domain.Entities.Order> CreateAsync(CreateOrderRequest request, CancellationToken ct)
    {
        var products = await Task.WhenAll(
            request.ProductItemModels.Select(f => catalogServiceClient.ChangeProductQuantityAsync(f.Id,f.Quantity, ct))
        );

        var productItems = products.ToList();
        
        if (productItems == null || !productItems.Any())
        {
            throw new EmptyProductsException();
        }

        logger.LogInformation("Запуск метода CreateAsync для списка продуктов: {productItems}",
            productItems.Select(f => f.ProductId));

        await productItemsRepository.AddRangeAsync(productItems, ct);
        var newOrder = await orderRepository.CreateAsync(productItems, ct);

        logger.LogInformation("Успешное завершение CreateAsync для списка продуктов: {productItems}",
            productItems.Select(f => f.ProductId));
        return newOrder;
    }
}