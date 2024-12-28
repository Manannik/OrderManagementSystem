namespace OrderProcessingService.Domain.Exceptions;

public class InvalidOrderStageException : InvalidOperationException
{
    public InvalidOrderStageException(string message) : base(message) { }
}