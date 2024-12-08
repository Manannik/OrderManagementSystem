namespace Domain.Exceptions
{
    public class ProductAlreadyExistException(string name)
        : CatalogServiceException($"Продукт с таким наименованием {name} уже существует", 409);
}