namespace Order.Domain.Exceptions;

public class CatalogServiceException(Guid id, string message, int statusCode) : Exception
{
    public Guid Id { get; set; } = id;
    public string Message { get; set; }= message;
    public int StatusCode { get; set; } = statusCode;

}