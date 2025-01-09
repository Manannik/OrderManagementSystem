using FluentValidation;

namespace Application.BusinessLogic.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
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
        
        RuleFor(f => f.CategoryModelDtos)
            .NotEmpty();
    }
    }
}