namespace Domain.Exceptions
{
    public class QuantityException() 
        : CatalogServiceException($"Нельзя задать количество товара менее 0", 409);
}