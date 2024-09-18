using FluentValidation;
using MediatR.Pipeline;

namespace WebApplication1;

public class GenericRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
{
    private readonly IValidator<TRequest> _validator;
    private readonly ILogger<GenericRequestPreProcessor<TRequest>> _logger;

    public GenericRequestPreProcessor(IValidator<TRequest> validator, 
        ILogger<GenericRequestPreProcessor<TRequest>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Ошибка валидации при обработке запроса {RequestName}: {@Errors}", typeof(TRequest).Name, validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }
    }
}