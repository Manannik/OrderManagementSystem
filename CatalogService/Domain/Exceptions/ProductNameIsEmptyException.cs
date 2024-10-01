namespace Domain.Exceptions;

public class ProductNameIsEmptyException() 
    : CatalogServiceException($"Нельзя создать продукт с пустым именем", 404);

