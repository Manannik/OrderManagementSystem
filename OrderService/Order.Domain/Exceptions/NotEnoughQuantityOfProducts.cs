namespace Order.Domain.Exceptions;

public class NotEnoughQuantityOfProducts()
    : OrderServiceException($"Недостаточно товаров на складе", 400);