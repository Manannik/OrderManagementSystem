﻿using Microsoft.Extensions.Logging;
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
        ICatalogService catalogService) : IOrderService
    {
        public async Task<OrderModelResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct)
        {
            logger.LogInformation("Запуск метода CreateAsync для списка продуктов: {ProductItemModels}",
                request.ProductItemModels);
            
            var productItemModels = request.ProductItemModels.ToList();
            
            var result = await catalogService.TryChangeQuantityAsync(productItemModels, ct);
             
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