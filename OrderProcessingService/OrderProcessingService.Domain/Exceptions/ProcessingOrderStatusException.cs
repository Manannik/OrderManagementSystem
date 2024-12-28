using OrderProcessingService.Domain.Enums;
using OrderProcessingService.Domain.Extensions;

namespace OrderProcessingService.Domain.Exceptions;

public class ProcessingOrderStatusException(string id, ProcessingOrderStatus orderStatus)
    : OrderProcessingException($"Заказ {id} находится в статусе {orderStatus.GetDisplayName()}", 409);