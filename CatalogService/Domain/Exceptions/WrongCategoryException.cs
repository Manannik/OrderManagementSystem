
namespace Domain.Exceptions;

public class WrongCategoryException : CatalogServiceException
{
    public List<string> Names { get; set; }

    public WrongCategoryException(List<string> names) : 
        base($"Категории с таким наименованиями {string.Join(", ", names)} не существует", 409)
    {
        Names = names;
    }
}

