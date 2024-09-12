using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Persistance;

namespace OrderManagementSystem.Infrastructure.Repository;

public class ProductRepositoryy(CatalogDbContext dbContext) : IProductRepository
{
    public async Task CreateAsync(Product product, CancellationToken ct)
    {
        var result = await dbContext.AddAsync(product, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await dbContext.Products.Include(f=>f.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Product product, CancellationToken ct)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistAsync(string name, CancellationToken ct)
    {
        return await dbContext.Products.FirstOrDefaultAsync(f => f.Name.ToLower() == name.ToLower()) != null;
    }

    public async Task UpdateQuantityAsync(Product product, CancellationToken ct)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(ct);
    }
}