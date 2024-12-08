using Order.Application.Enums;

namespace Order.Application.Models;

public class OrderModelResponse
{
    public Guid Id { get; set; }
    public decimal Cost { get; set; }
    public OrderStatusModel OrderStatus { get; set; }
}