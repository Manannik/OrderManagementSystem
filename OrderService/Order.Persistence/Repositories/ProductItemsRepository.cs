using Order.Domain.Abstractions;
using Order.Domain.Entities;

namespace Order.Persistence.Repositories
{
    public class ProductItemsRepository(OrderDbContext dbContext) : IProductItemsRepository
    {
        public async Task AddRangeAsync(List<ProductItem> productItems, Guid id, CancellationToken ct)
        {
            await dbContext.ProductItems.AddRangeAsync(productItems, ct);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}