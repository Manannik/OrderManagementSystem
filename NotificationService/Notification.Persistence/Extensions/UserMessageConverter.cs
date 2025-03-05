using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Notification.Domain.Entities.User;

namespace Notification.Persistence.Extensions;

public class UserMessageConverter : ValueConverter<UserMessage?, int?>
{
    public UserMessageConverter()
        : base(
            um => um.Id,
            id => id == null ? null : new UserMessage(id.Value)
        )
    {
    }
}