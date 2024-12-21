using Order.Domain.Enums;

namespace Order.Domain.Services;

public static class OrderStatusTransitions
{
    private static readonly Dictionary<OrderStatus, List<OrderStatus>> AllowedTransitions = new()
    {
        { OrderStatus.Created, new List<OrderStatus> { OrderStatus.InProgress, OrderStatus.Cancelled } },
        { OrderStatus.InProgress, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
        { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
        { OrderStatus.Cancelled, new List<OrderStatus>() },
        { OrderStatus.Delivered, new List<OrderStatus>() }
    };

    public static bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return AllowedTransitions.ContainsKey(currentStatus) && 
               AllowedTransitions[currentStatus].Contains(newStatus);
    }
    

}