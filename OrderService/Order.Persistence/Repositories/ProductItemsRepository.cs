using Microsoft.EntityFrameworkCore;
using Order.Domain.Abstarctions;
using Order.Domain.Entities;

namespace Order.Persistence.Repositories
{
    public class ProductItemsRepository(OrderDbContext dbContext) : IProductItemsRepository
    {
        public async Task AddRangeAsync(List<ProductItem> productItems, CancellationToken ct)
    {
        var existingProductItems =
            await dbContext.ProductItems.Where(f => productItems.Select(g => g.ProductId).Contains(f.ProductId))
                .ToListAsync(ct);

        foreach (var existingProductItem in existingProductItems)
        {
            var newItem = productItems.FirstOrDefault(pi => pi.ProductId == existingProductItem.ProductId);
            if (newItem != null)
            {
                newItem.Quantity += existingProductItem.Quantity;
                productItems.Remove(newItem);
            }
        }
        
        if (productItems.Count != 0)
        {
            await dbContext.ProductItems.AddRangeAsync(productItems, ct);
        }
        
        await dbContext.SaveChangesAsync(ct);
    }
    }
}