namespace Domain.Exceptions;

public class ProductDoesNotExistException : CatalogServiceException
{
    public Guid Id { get; set; }
    public ProductDoesNotExistException(Guid id) : base($"Продукт с id = {id} не найден", 404)
    {
        Id = id;
    }
}

