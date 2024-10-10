
namespace Domain.Exceptions;

public class WrongCategoryException(List<Guid> ids)
    : CatalogServiceException($"Категории с такими Id {string.Join(", ", ids)} не существует", 409);

