
namespace Domain.Exceptions;

public class WrongCategoryException : CatalogServiceException
{
    public List<Guid> Ids { get; set; }

    public WrongCategoryException(List<Guid> ids) : 
        base($"Категории с такими Id {string.Join(", ", ids)} не существует", 409)
    {
        Ids = ids;
    }
}

