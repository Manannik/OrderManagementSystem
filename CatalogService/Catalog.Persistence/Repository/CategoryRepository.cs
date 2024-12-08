using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderManagementSystem.Infrastructure.Repository
{
    public class CategoryRepository(CatalogDbContext dbContext) : ICategoryRepository
    {
        public Task<Guid> CreateAsync(Category category, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

        public async Task<List<Category>> GetByIdAsync(List<Guid> ids, CancellationToken ct)
    {
        return await dbContext.Categories.Where(f=>ids.Contains(f.Id)).ToListAsync(ct);
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
}