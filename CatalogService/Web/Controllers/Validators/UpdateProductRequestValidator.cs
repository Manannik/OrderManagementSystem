using Application.Models;
using FluentValidation;

namespace WebApplication1.Controllers.Validators;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(f => f.Price)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(f => f.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100);
        
        RuleFor(f => f.Description)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(100);

        RuleFor(f => f.Quantity)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(f => f.Category)
            .NotEmpty();
    }
}