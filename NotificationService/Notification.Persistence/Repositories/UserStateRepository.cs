using Microsoft.EntityFrameworkCore;
using Notification.Domain.Abstractions;
using Notification.Domain.Entities.User;

namespace Notification.Persistence.Repositories;

public class UserStateRepository(NotificationDbContext notificationDbContext) : IUserStateRepository
{
    public async Task CreateAsync(UserState userState, CancellationToken cancellationToken)
    {
        await notificationDbContext.UserStates.AddAsync(userState, cancellationToken);
        await notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserState?> TryGetByTelegramIdAsync(long id, CancellationToken cancellationToken)
    {
        return await notificationDbContext.UserStates.Include(f=>f.UserData)
            .FirstOrDefaultAsync(f => f.TelegramUserId == id,cancellationToken);
    }

    public async Task<UserState?> TryGetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await notificationDbContext.UserStates.FirstOrDefaultAsync(f => f.UserData.OrderId == orderId,cancellationToken);
    }

    public async Task UpdateAsync(UserState userState, CancellationToken cancellationToken)
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
}