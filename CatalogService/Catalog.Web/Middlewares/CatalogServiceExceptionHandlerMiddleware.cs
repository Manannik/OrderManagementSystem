using Domain.Exceptions;

namespace WebApplication.Middlewares;

public class CatalogServiceExceptionHandlerMiddleware(ILogger<CatalogServiceExceptionHandlerMiddleware> _logger)
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
                case ProductAlreadyExistException productAlreadyExistException:
                    _logger.LogError("Ошибка создания продукта, такой продукт уже существует");
                    context.Response.StatusCode = productAlreadyExistException.StatusCode;
                    await context.Response.WriteAsync(productAlreadyExistException.Message);
                    break;

                case ProductDoesNotExistException productDoesNotExistException:
                    _logger.LogError("Ошибка выбора продукта, такого продукта не существует");
                    context.Response.StatusCode = productDoesNotExistException.StatusCode;
                    await context.Response.WriteAsync(productDoesNotExistException.Message);
                    break;

                case WrongCategoryException wrongCategoryException:
                    _logger.LogError("Ошибка категории продукта, попытка создать продукт с несуществующей категорией");
                    context.Response.StatusCode = wrongCategoryException.StatusCode;
                    await context.Response.WriteAsync(wrongCategoryException.Message);
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