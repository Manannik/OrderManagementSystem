using Application.Models;
using FluentValidation;

namespace WebApplication1.Controllers.Validators;

public class UpdateProductQuantityRequestValidator:AbstractValidator<UpdateProductQuantityRequest>
{
    public UpdateProductQuantityRequestValidator()
    {
        RuleFor(f => f.NewQuantity)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);
    }
}