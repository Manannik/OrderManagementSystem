using Application.Models;
using FluentValidation;
using MediatR.Pipeline;

namespace WebApplication1;

public class CreateProductRequestPreProcessor : IRequestPreProcessor<CreateProductRequest>
{
    private readonly IValidator<CreateProductRequest> _validator;

    public CreateProductRequestPreProcessor(IValidator<CreateProductRequest> validator)
    {
        _validator = validator;
    }

    public async Task Process(CreateProductRequest request, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}