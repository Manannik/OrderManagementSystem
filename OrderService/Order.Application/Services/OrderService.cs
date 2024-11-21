using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Application.Enums;
using Order.Application.Helpers;
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
        ICatalogServiceClient catalogServiceClient,
        IKafkaProducer<CreateOrderKafkaModel> createOrderProducer,
        IKafkaProducer<UpdatedOrderKafkaModel> updatedOrderProducer) : IOrderService
    {
        public async Task<OrderModelResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct)
        {
            logger.LogInformation("Запуск метода CreateAsync для списка продуктов: {ProductItemModels}",
                request.ProductItemModels);
            
            var productItemModels = request.ProductItemModels.ToList();
            ValidateProductsItems(productItemModels);

            var result = await TryChangeQuantityAsync(productItemModels, catalogServiceClient, ct);
            if (!result.IsSuccess)
            {
                logger.LogWarning("Обнаружены ошибки: {Errors}", result.Errors);
                throw new AggregateException(result.Errors.Select(e => 
                    new CatalogServiceException(e.id, e.Message, e.StatusCode)));
            }

            var productItems = result.Value;
            var newOrder = await orderRepository.CreateAsync(productItems, ct);

            var orderKafkaModel = new CreateOrderKafkaModel()
            {
                Id = newOrder.Id,
                OrderStatus = newOrder.OrderStatus.ToString(),
                Cost = newOrder.Cost,
                CreatedAt = DateTime.UtcNow
            };
            
            await createOrderProducer.ProduceAsync(orderKafkaModel, ct);
            
            var newOrderResponse = new OrderModelResponse()
            {
                Id = newOrder.Id,
                OrderStatus = (OrderStatusModel) newOrder.OrderStatus,
                Cost = newOrder.Cost
            };
            
            logger.LogInformation("Успешное завершение CreateAsync для списка продуктов: {productItems}",
                productItems.Select(f => f.ProductId));
            return newOrderResponse;
        }

        public async Task<OrderModelResponse> UpdateAsync(ChangeOrderStatusRequest request, CancellationToken ct)
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
            
            var updatedOrderKafkaModel = new UpdatedOrderKafkaModel()
            {
                Id = updatedOrder.Id,
                OrderStatus = updatedOrder.OrderStatus.ToString(),
                Cost = updatedOrder.Cost,
                UpdatedAt = DateTime.UtcNow
            };
            
            await updatedOrderProducer.ProduceAsync(updatedOrderKafkaModel, ct);
            
            var updatedOrderResponse = new OrderModelResponse()
            {
                Id = updatedOrder.Id,
                OrderStatus = (OrderStatusModel)updatedOrder.OrderStatus,
                Cost = updatedOrder.Cost
            };
            return updatedOrderResponse;
        }

        private void ValidateProductsItems(List<ProductItemModel> productItemModels)
        {
            if (productItemModels == null || !productItemModels.Any())
            {
                throw new EmptyProductsException();
            }
        }

        private async Task<Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>> TryChangeQuantityAsync(
            IEnumerable<ProductItemModel> productItemModels,
            ICatalogServiceClient catalogServiceClient,
            CancellationToken ct)
        {
            var errors = new ConcurrentBag<(Guid id, string Message, int StatusCode)>();
            var productItems = new ConcurrentBag<ProductItem>();

            var tasks = productItemModels.Select(async model =>
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

            if (errors.IsEmpty)
            {
                return Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>.Success(productItems.ToList());
            }

            return Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>.Failure(errors.ToList());
        }

    }
}