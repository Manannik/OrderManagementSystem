namespace Order.Application.Abstractions
{
    public interface ICatalogServiceClient
    {
        Task<bool> ChangeProductQuantityAsync(Guid id, int newQuantity,decimal price, CancellationToken ct);
        Task<bool> ProductExistsAsync(Guid id, CancellationToken ct);
    }
}