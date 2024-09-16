using Domain.Entities;

namespace Domain.Abstractions;

public interface ICategoryRepository
{
    public Task<Guid> CreateAsync(Category category, CancellationToken ct);
    public Task<List<Category>> GetByIdAsync(List<Guid> names, CancellationToken ct);
    public Task<Category> UpdateAsync(Category category, CancellationToken ct);
    public Task DeleteAsync(Guid id, CancellationToken ct);
}