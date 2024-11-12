using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        IKafkaProducer _producer) : IOrderService
    {
        public async Task<Domain.Entities.Order> CreateAsync(CreateOrderRequest request, CancellationToken ct)
        {
            logger.LogInformation("Запуск метода CreateAsync для списка продуктов: {ProductItemModels}",
                request.ProductItemModels);

            foreach (var model in request.ProductItemModels)
            {
                var result =
                    await catalogServiceClient.ChangeProductQuantityAsync(model.Id, model.Quantity, model.Price, ct);
                if (!result)
                {
                    throw new ProductException();
                }
            }

            var products = await Task.WhenAll(
                request.ProductItemModels.Select(f =>
                    catalogServiceClient.ChangeProductQuantityAsync(f.Id, f.Quantity, f.Price, ct))
            );

            var productItemModels = request.ProductItemModels.ToList();

            if (productItemModels == null || !productItemModels.Any())
            {
                throw new EmptyProductsException();
            }

            var productItems = productItemModels.Select(f => new ProductItem()
            {
                ProductId = f.Id,
                Quantity = f.Quantity,
                Price = f.Price
            }).ToList();

            await productItemsRepository.AddRangeAsync(productItems, ct);
            var newOrder = await orderRepository.CreateAsync(productItems, ct);

            logger.LogInformation("Успешное завершение CreateAsync для списка продуктов: {productItems}",
                productItems.Select(f => f.ProductId));

            await _producer.ProduceAsync("order-topic", new Message<string, string>()
            {
                Key = newOrder.Id.ToString(),
                Value = JsonConvert.SerializeObject(newOrder)
            });

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
            return updatedOrder;
        }
    }
}