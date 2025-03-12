using Microsoft.EntityFrameworkCore;
using Notification.Domain.Abstractions;
using Notification.Domain.Entities.User;

namespace Notification.Persistence.Repositories;

public class UserDataRepository(NotificationDbContext notificationDbContext) : IUserDataRepository
{
    public async Task CreateAsync(UserData userData, CancellationToken cancellationToken)
    {
        await notificationDbContext.UserDatas.AddAsync(userData, cancellationToken);
        await notificationDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserData?> TryGetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await notificationDbContext.UserDatas
            .FirstOrDefaultAsync(f => f.OrderId == orderId, cancellationToken);
    }

    public async Task UpdateAsync(UserData userData, CancellationToken cancellationToken)
    {
        notificationDbContext.UserDatas.Update(userData);
        await notificationDbContext.SaveChangesAsync(cancellationToken);
    }
}