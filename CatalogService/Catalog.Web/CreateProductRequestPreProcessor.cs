using Application.BusinessLogic.Commands.CreateProduct;
using Application.Models;
using FluentValidation;
using MediatR.Pipeline;

namespace WebApplication1
{
    public class CreateProductRequestPreProcessor(IValidator<CreateProductCommand> validator)
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