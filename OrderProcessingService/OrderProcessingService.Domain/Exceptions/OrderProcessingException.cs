namespace OrderProcessingService.Domain.Exceptions;

public class OrderProcessingException(string message, int statusCode) : Exception
{
    public string Message { get; set; } = message;

    public int StatusCode { get; set; } = statusCode;
}