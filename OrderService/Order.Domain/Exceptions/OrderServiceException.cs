namespace Order.Domain.Exceptions
{
    public class OrderServiceException(string message, int statusCode) : Exception
    {
        public string Message { get; set; } = message;

        public int StatusCode { get; set; } = statusCode;
    }
}