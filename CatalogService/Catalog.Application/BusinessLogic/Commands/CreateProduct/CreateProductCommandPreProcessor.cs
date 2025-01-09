using FluentValidation;
using MediatR.Pipeline;

namespace Application.BusinessLogic.Commands.CreateProduct
{
    public class CreateProductCommandPreProcessor(IValidator<CreateProductCommand> validator)
        : IRequestPreProcessor<CreateProductCommand>
    {
        public async Task Process(CreateProductCommand request, CancellationToken ct)
        {
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}