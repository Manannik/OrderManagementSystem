using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Persistance;

namespace OrderManagementSystem.Infrastructure.Repository;

public class CategoryRepository(CatalogDbContext dbContext) : ICategoryRepository
{
    public Task<Guid> CreateAsync(Category category, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken ct)
    {
        return await dbContext.Categories.FirstOrDefaultAsync(f => f.Name.ToLower() == name.ToLower());
    }

    public Task<Category> UpdateAsync(Category category, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}