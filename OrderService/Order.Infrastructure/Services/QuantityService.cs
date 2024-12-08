using System.Collections.Concurrent;
using Order.Application.Abstractions;
using Order.Application.Helpers;
using Order.Application.Models;
using Order.Domain.Entities;
using Order.Domain.Exceptions;

namespace Order.Infrastructure.Services;

public class QuantityService : IQuantityService
{
    private readonly ICatalogServiceClient _catalogServiceClient;

    public QuantityService(ICatalogServiceClient catalogServiceClient)
    {
        _catalogServiceClient = catalogServiceClient;
    }

    public async Task<Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>> TryChangeQuantityAsync
    (IEnumerable<ProductItemModel> productItemModels, CancellationToken ct)
    {
        var errors = new ConcurrentBag<(Guid id, string Message, int StatusCode)>();
        var productItems = new ConcurrentBag<ProductItem>();

        var tasks = productItemModels.Select(async model =>
        {
            try
            {
                var result = await _catalogServiceClient.ChangeProductQuantityAsync(model.Id, model.Quantity, ct);

                productItems.Add(new ProductItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = model.Id,
                    Quantity = model.Quantity,
                    Price = result.Price
                });
            }
            catch (CatalogServiceException ex)
            {
                errors.Add((model.Id, ex.Message, ex.StatusCode));
            }
        }).ToList();

        await Task.WhenAll(tasks);

        if (errors.IsEmpty)
        {
            return Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>.Success(productItems.ToList());
        }

        return Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>.Failure(errors.ToList());
    }
}