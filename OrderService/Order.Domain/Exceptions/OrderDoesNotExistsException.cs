namespace Order.Domain.Exceptions;

public class OrderDoesNotExistsException (string id)
    : OrderServiceException($"Заказа с id {id} не найден", 402);
