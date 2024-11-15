using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Application.Models;
using Order.Domain.Abstractions;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Domain.Exceptions;

namespace Order.Application.Services
{
    public class OrderService(
        ILogger<OrderService> logger,
        IOrderRepository orderRepository,
        IProductItemsRepository productItemsRepository,
        ICatalogServiceClient catalogServiceClient,
        IKafkaProducer<Domain.Entities.Order> producer) : IOrderService
    {
        public async Task<Domain.Entities.Order> CreateAsync(CreateOrderRequest request, CancellationToken ct)
        {
            logger.LogInformation("Запуск метода CreateAsync для списка продуктов: {ProductItemModels}",
                request.ProductItemModels);

            var productItemModels = request.ProductItemModels.ToList();

            if (productItemModels == null || !productItemModels.Any())
            {
                throw new EmptyProductsException();
            }

            var errors = new ConcurrentBag<(Guid id, string Message, int StatusCode)>();
            var productItems = new List<ProductItem>();

            var tasks = request.ProductItemModels.Select(async model =>
            {
                try
                {
                    var result = await catalogServiceClient.ChangeProductQuantityAsync(model.Id, model.Quantity, ct);

                    productItems.Add(new ProductItem
                    {
                        ProductId = model.Id,
                        Quantity = model.Quantity,
                        Price = result.Price
                    });
                }
                catch (CatalogServiceException ex)
                {
                    errors.Add((model.Id, ex.Message, ex.StatusCode));
                }
            }).ToList();

            await Task.WhenAll(tasks);

            if (!errors.IsEmpty)
            {
                logger.LogWarning("Обнаружены ошибки {Errors}", errors);
                throw new AggregateException(errors.Select(e =>
                    new CatalogServiceException(e.id, e.Message, e.StatusCode)));
            }

            var newOrder = await orderRepository.CreateAsync(productItems, ct);
            productItems.ForEach(item =>
            {
                item.OrderId = newOrder.Id;
                item.Order = newOrder;
            });

            await productItemsRepository.AddRangeAsync(productItems, newOrder.Id, ct);

            logger.LogInformation("Успешное завершение CreateAsync для списка продуктов: {productItems}",
                productItems.Select(f => f.ProductId));

            await producer.ProduceAsync(newOrder, ct);

            return newOrder;
        }

        public async Task<Domain.Entities.Order> UpdateAsync(ChangeOrderStatusRequest request, CancellationToken ct)
        {
            logger.LogInformation("Запуск метода UpdateAsync для заказа: {Id}",
                request.Id);
            var existingOrder = await orderRepository.GetByIdAsync(request.Id, ct);
            if (existingOrder == null)
            {
                throw new OrderDoesNotExistsException(request.Id.ToString());
            }
            var orderStatus = (OrderStatus)request.OrderStatusModel;
            var updatedOrder = await orderRepository.UpdateStatusAsync(existingOrder, orderStatus, ct);
            return updatedOrder!;
        }
    }
}