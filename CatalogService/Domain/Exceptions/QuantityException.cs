namespace Domain.Exceptions;

public class QuantityException : CatalogServiceException
{
    public QuantityException() : base($"Нельзя задать количество товара менее 0", 409)
    {
    }
}