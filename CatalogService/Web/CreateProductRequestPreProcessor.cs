using Application.Models;
using FluentValidation;
using MediatR.Pipeline;

namespace WebApplication1;

public class CreateProductRequestPreProcessor(IValidator<CreateProductRequest> validator)
    : IRequestPreProcessor<CreateProductRequest>
{
    public async Task Process(CreateProductRequest request, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}