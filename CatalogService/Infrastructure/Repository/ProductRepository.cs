using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
        return await dbContext.Products.Include(f=>f.Categories)
            .FirstOrDefaultAsync(p => p.Id == id,ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Product product, CancellationToken ct)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistAsync(string name, CancellationToken ct)
    {
        return await dbContext.Products.AnyAsync(f=>f.Name==name,ct);
    }

    public async Task UpdateQuantityAsync(Product product, CancellationToken ct)
    {
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(ct);
    }
}