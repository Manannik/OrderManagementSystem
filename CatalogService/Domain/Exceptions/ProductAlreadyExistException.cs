namespace Domain.Exceptions;

public class ProductAlreadyExistException : CatalogServiceException
{
    public string Name { get; set; }

    public ProductAlreadyExistException(string name) : base($"Продукт с таким наименованием {name} уже существует", 409)
    {
        Name = name;
    }
}