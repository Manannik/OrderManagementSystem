using Notification.Domain.Entities.User;

namespace Notification.Domain.Abstractions;

public interface INotificationRepository
{
    Task CreateUserStateAsync(UserState userState, CancellationToken cancellationToken);
    Task<UserState?> TryGetUserStateByTelegramIdAsync(long id, CancellationToken cancellationToken);
    Task<UserState?> TryGetUserStateByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateUserStateAsync(UserState userState, CancellationToken cancellationToken);
    Task CreateUserDataAsync(UserData userData, CancellationToken cancellationToken);
    Task<UserData?> TryGetUserDataByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateUserDataAsync(UserData userData, CancellationToken cancellationToken);
}