
namespace Domain.Exceptions;

public class WrongCategoryException : CatalogServiceException
{
    public string Name { get; set; }

    public WrongCategoryException(string name) : base($"Категории с таким наименованием {name} не существует", 409)
    {
        Name = name;
    }
}

