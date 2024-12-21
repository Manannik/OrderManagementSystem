using Order.Application.Helpers;
using Order.Application.Models;
using Order.Domain.Entities;

namespace Order.Application.Abstractions;

public interface ICatalogService
{
    Task<Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>> TryChangeQuantityAsync(
        IEnumerable<ProductItemModel> productItemModels,
        CancellationToken ct);
}