using Microsoft.EntityFrameworkCore;
using Notification.Domain.Abstractions;
using Notification.Domain.Entities.User;

namespace Notification.Persistence.Repositories;

public class NotificationRepository(NotificationDbContext notificationDbContext) : INotificationRepository
{
    public async Task CreateUserStateAsync(UserState userState, CancellationToken cancellationToken)
    {
        await notificationDbContext.UserStates.AddAsync(userState, cancellationToken);
        await notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserState?> TryGetUserStateByTelegramIdAsync(long id, CancellationToken cancellationToken)
    {
        return await notificationDbContext.UserStates.Include(f=>f.UserData)
            .FirstOrDefaultAsync(f => f.TelegramUserId == id,cancellationToken);
    }

    public async Task<UserState?> TryGetUserStateByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await notificationDbContext.UserStates.FirstOrDefaultAsync(f => f.UserData.OrderId == orderId,cancellationToken);
    }

    public async Task UpdateUserStateAsync(UserState userState, CancellationToken cancellationToken)
    {
        var existingUserState = await notificationDbContext.UserStates
            .FirstOrDefaultAsync(us => us.TelegramUserId == userState.TelegramUserId, cancellationToken);

        if (existingUserState != null)
        {
            existingUserState.Pages = userState.Pages;
            existingUserState.LastUserMessage = userState.LastUserMessage;
            await notificationDbContext.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task CreateUserDataAsync(UserData userData, CancellationToken cancellationToken)
    {
        await notificationDbContext.UserDatas.AddAsync(userData, cancellationToken);
        await notificationDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<UserData?> TryGetUserDataByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await notificationDbContext.UserDatas
            .FirstOrDefaultAsync(f => f.OrderId == orderId, cancellationToken);
    }

    public async Task UpdateUserDataAsync(UserData userData, CancellationToken cancellationToken)
    {
        notificationDbContext.UserDatas.Update(userData);
        await notificationDbContext.SaveChangesAsync(cancellationToken);
    }
}