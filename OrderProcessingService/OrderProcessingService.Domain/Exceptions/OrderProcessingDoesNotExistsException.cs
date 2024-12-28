namespace OrderProcessingService.Domain.Exceptions;

public class OrderProcessingDoesNotExistsException (string id)
    : OrderProcessingException($"Заказа с id {id} не найден", 402);
