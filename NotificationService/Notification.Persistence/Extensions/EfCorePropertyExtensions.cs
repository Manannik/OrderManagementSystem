using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notification.Persistence.Extensions;

public static class EfCorePropertyExtensions
{
    public static PropertyBuilder<List<TValueObject>> JsonValueObjectCollectionConversion<TValueObject>(
        this PropertyBuilder<List<TValueObject>> builder)
    {
        return builder.HasConversion(
            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
            v => JsonSerializer.Deserialize<List<TValueObject>>(v, JsonSerializerOptions.Default) ?? new List<TValueObject>(),

            new ValueComparer<IReadOnlyList<TValueObject>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
                c => c.ToList()
            )
        );
    }
}