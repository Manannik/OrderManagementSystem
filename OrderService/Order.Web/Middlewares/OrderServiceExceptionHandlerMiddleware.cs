using Order.Domain.Exceptions;

namespace Order.Web.Middlewares
{
    public class OrderServiceExceptionHandlerMiddleware(ILogger<OrderServiceExceptionHandlerMiddleware> _logger)
        : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case EmptyProductsException emptyProductsException:
                        _logger.LogError("Попытка создать заказ с пустым списком Products");
                        context.Response.StatusCode = emptyProductsException.StatusCode;
                        await context.Response.WriteAsync(emptyProductsException.Message);
                        break;
                    case ProductException notEnoughQuantityOfProducts:
                        _logger.LogError("Попытка создать заказ с пустым списком Products");
                        context.Response.StatusCode = notEnoughQuantityOfProducts.StatusCode;
                        await context.Response.WriteAsync(notEnoughQuantityOfProducts.Message);
                        break;
                    case OrderDoesNotExistsException doesNotExistsException:
                        _logger.LogError($"Попытка получить несуществующий заказ");
                        context.Response.StatusCode = doesNotExistsException.StatusCode;
                        await context.Response.WriteAsync(doesNotExistsException.Message);
                        break;
                    default:
                        context.Response.StatusCode = 500;
                        _logger.LogError("что то пошло не так");
                        await context.Response.WriteAsync("что то пошло не так");
                        break;
                }
            }
        }
    }
}