namespace Domain.Exceptions;

public class CatalogServiceException(string message, int statusCode) : Exception
{
    public string Message { get; set; } = message;

    public int StatusCode { get; set; } = statusCode;
}