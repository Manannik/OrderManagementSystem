using FluentValidation;
using Order.Application.Enums;
using Order.Application.Requests;
using Order.Domain.Enums;
using Order.Domain.Services;

namespace Order.Web.Controllers.Validators;

public class ChangeOrderStatusRequestValidator : AbstractValidator<ChangeOrderStatusRequest>
{
    public ChangeOrderStatusRequestValidator()
    {
        RuleFor(request => request.OrderStatusModel)
            .IsInEnum()
            .WithMessage("Неверный статус заказа. Статус заказа должен быть в диапазоне значений OrderStatusModel.");
        
        RuleFor(request => request.OrderStatusModel)
            .Must((request, newStatus) => OrderStatusTransitions
                .IsValidTransition(MapOrderStatusModelToDomain(request.OrderStatusModel), MapOrderStatusModelToDomain(newStatus)))
            .WithMessage("Ошибка при попытке изменить статус заказа.");
    }
    
    private OrderStatus MapOrderStatusModelToDomain(OrderStatusModel model)
    {
        return model switch
        {
            OrderStatusModel.Created => OrderStatus.Created,
            OrderStatusModel.InProgress => OrderStatus.InProgress,
            OrderStatusModel.Shipped => OrderStatus.Shipped,
            OrderStatusModel.Delivered => OrderStatus.Delivered,
            OrderStatusModel.Cancelled => OrderStatus.Cancelled,
            _ => throw new InvalidOperationException("Неизвестный статус заказа")
        };
    }
}