using Notification.Domain.Entities.User;

namespace Notification.Domain.Abstractions;

public interface IUserStateRepository
{
    Task CreateAsync(UserState userState, CancellationToken cancellationToken);
    Task<UserState?> TryGetByTelegramIdAsync(long id, CancellationToken cancellationToken);
    Task<UserState?> TryGetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateAsync(UserState userState, CancellationToken cancellationToken);
}