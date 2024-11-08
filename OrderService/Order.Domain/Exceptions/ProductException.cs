namespace Order.Domain.Exceptions;

public class ProductException()
    : OrderServiceException($"Проблема с товаром на скаде", 400);