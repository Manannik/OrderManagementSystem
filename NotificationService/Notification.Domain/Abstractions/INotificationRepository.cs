using Notification.Domain.Entities.User;

namespace Notification.Domain.Abstractions;

public interface INotificationRepository
{
    Task CreateUserDataAsync(UserData userData, CancellationToken cancellationToken);
    Task CreateUserStateAsync(UserState userState, CancellationToken cancellationToken);
    Task<UserState?> TryGetUserStateByTelegramIdAsync(long id, CancellationToken cancellationToken);
    Task<UserData?> TryGetUserDataByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task UpdateUserDataAsync(UserData userData, CancellationToken cancellationToken);
    Task UpdateUserStateAsync(UserState userState, CancellationToken cancellationToken);
}