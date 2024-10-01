namespace Domain.Exceptions;

public class ProductDescriptionIsEmptyException() 
    : CatalogServiceException($"Нельзя создать продукт с пустым описанием", 404);

