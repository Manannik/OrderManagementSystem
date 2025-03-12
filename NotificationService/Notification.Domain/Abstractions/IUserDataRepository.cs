using Notification.Domain.Entities.User;

namespace Notification.Domain.Abstractions;

public interface IUserDataRepository
{
    Task CreateAsync(UserData userData, CancellationToken cancellationToken);
    Task<UserData?> TryGetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateAsync(UserData userData, CancellationToken cancellationToken);
}