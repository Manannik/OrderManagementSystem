using FluentValidation;
using Order.Application.Models;

namespace Order.Web.Controllers.Validators;

public class ChangeOrderStatusRequestValidator : AbstractValidator<ChangeOrderStatusRequest>
{
    public ChangeOrderStatusRequestValidator()
    {
        RuleFor(request => request.OrderStatusModel)
            .IsInEnum()
            .WithMessage("Неверный статус заказа. Статус заказа должен быть в диапазоне значений OrderStatusModel.");
    }
}