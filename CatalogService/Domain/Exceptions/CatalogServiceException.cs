namespace Domain.Exceptions;

public class CatalogServiceException : Exception
{
    public string Message { get; set; }
    
    public int StatusCode { get; set; }

    public CatalogServiceException(string message, int statusCode)
    {
        Message = message;
        StatusCode = statusCode;
    }
}