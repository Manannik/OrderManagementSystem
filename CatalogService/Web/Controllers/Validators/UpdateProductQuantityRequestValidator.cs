using Application.Models;
using FluentValidation;

namespace WebApplication1.Controllers.Validators;

public class UpdateProductQuantityRequestValidator:AbstractValidator<OrderedQuantity>
{
    public UpdateProductQuantityRequestValidator()
    {
        RuleFor(f => f.Quantity)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);
    }
}