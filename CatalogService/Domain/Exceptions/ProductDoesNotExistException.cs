namespace Domain.Exceptions;

public class ProductDoesNotExistException(Guid id) 
    : CatalogServiceException($"Продукт с id = {id} не найден", 404);

