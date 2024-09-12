using Domain.Entities;

namespace Domain.Abstractions;

public interface ICategoryRepository
{
    public Task<Guid> CreateAsync(Category category, CancellationToken ct);
    public Task<Category?> GetByNameAsync(string name, CancellationToken ct);
    public Task<Category> UpdateAsync(Category category, CancellationToken ct);
    public Task DeleteAsync(Guid id, CancellationToken ct);
}