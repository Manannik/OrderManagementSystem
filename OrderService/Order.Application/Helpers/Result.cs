namespace Order.Application.Helpers;

public class Result<TSuccess, TError>
{
    public TSuccess? Value { get; private set; }
    public List<TError> Errors { get; private set; } = new();
    public bool IsSuccess => Errors.Count == 0;

    private Result(TSuccess value)
    {
        Value = value;
    }

    private Result(List<TError> errors)
    {
        Errors = errors;
    }

    public static Result<TSuccess, TError> Success(TSuccess value) => new(value);
    public static Result<TSuccess, TError> Failure(List<TError> errors) => new(errors);
}
