using Order.Application.Enums;

namespace Order.Application.Models;

public class ChangeOrderStatusRequest
{
    public Guid Id { get; set; }
    public OrderStatusModel OrderStatusModel { get; set; }
}