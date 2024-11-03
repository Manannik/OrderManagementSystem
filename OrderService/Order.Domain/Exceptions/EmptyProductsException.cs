namespace Order.Domain.Exceptions;

public class EmptyProductsException() 
    : OrderServiceException($"Попытка создать заказ с пустым списком продуктов", 401);
