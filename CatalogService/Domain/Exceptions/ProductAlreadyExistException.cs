namespace Domain.Exceptions;

public class ProductAlreadyExistException : CatalogServiceException
{

    public ProductAlreadyExistException(string name) : base($"Продукт с таким наименованием {name} уже существует", 409)
    {
    }
}