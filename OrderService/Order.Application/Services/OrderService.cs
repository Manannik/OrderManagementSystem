using Microsoft.Extensions.Logging;
using Order.Application.Abstractions;
using Order.Application.Enums;
using Order.Application.Models;
using Order.Application.Models.Kafka;
using Order.Application.Requests;
using Order.Domain.Abstractions;
using Order.Domain.Enums;
using Order.Domain.Exceptions;

namespace Order.Application.Services
{
    public class OrderService(
        ILogger<OrderService> logger,
        IOrderRepository orderRepository,
        IKafkaProducer<CreateOrderKafkaModel> createOrderProducer,
        IKafkaProducer<UpdatedOrderKafkaModel> updatedOrderProducer,
        IQuantityService quantityService) : IOrderService
    {
        public async Task<OrderModelResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct)
        {
            logger.LogInformation("Запуск метода CreateAsync для списка продуктов: {ProductItemModels}",
                request.ProductItemModels);
            
            var productItemModels = request.ProductItemModels.ToList();
            
            
            
            var result = await quantityService.TryChangeQuantityAsync(productItemModels, ct);
            /*
             вынес проверку в валидатор, но не могу избавиться от повтроного вызора TryChangeQuantityAsync
             кажется что сделал только хуже эти т.к. 2 раза вызываю
             
            if (!result.IsSuccess)
            {
                logger.LogWarning("Обнаружены ошибки: {Errors}", result.Errors);
                throw new AggregateException(result.Errors.Select(e => 
                    new CatalogServiceException(e.id, e.Message, e.StatusCode)));
            }
            */
            var productItems = result.Value;
            var newOrder = await orderRepository.CreateAsync(productItems, ct);

            var orderKafkaModel = new CreateOrderKafkaModel()
            {
                Id = newOrder.Id,
                OrderStatus = newOrder.OrderStatus.ToString(),
                Cost = newOrder.Cost,
                CreatedAt = DateTime.UtcNow,
                ProductItemModels = newOrder.ProductItems.Select(f=>new ProductItemModel()
                {
                    Id = f.Id,
                    Quantity = f.Quantity
                }).ToList()
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
                logger.LogInformation("Заказ с указанный Id: {Id} отсутствует",
                    request.Id);
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
            logger.LogInformation("Успешное завершение UpdateAsync для заказа: {order}",
                updatedOrderResponse);
            return updatedOrderResponse;
        }
    }
}