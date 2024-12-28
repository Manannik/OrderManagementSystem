namespace OrderProcessingService.Domain.Exceptions;

public class InvalidOrderStatusTransitionException : InvalidOperationException
{
    public InvalidOrderStatusTransitionException(string message) : base(message) { }
}