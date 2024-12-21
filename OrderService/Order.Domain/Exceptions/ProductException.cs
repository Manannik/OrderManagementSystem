namespace Order.Domain.Exceptions
{
    public class ProductException()
        : OrderServiceException($"Проблема с товаром на складе", 400);
}