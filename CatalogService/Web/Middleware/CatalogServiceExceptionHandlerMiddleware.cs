using Domain.Exceptions;

namespace WebApplication1.Middleware;

public class CatalogServiceExceptionHandlerMiddleware: IMiddleware
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
                    context.Response.StatusCode = productAlreadyExistException.StatusCode;
                    await context.Response.WriteAsync(productAlreadyExistException.Message);
                    break;

                case ProductDoesNotExistException productDoesNotExistException:
                    context.Response.StatusCode = productDoesNotExistException.StatusCode;
                    await context.Response.WriteAsync(productDoesNotExistException.Message);
                    break;

                case WrongCategoryException wrongCategoryException:
                    context.Response.StatusCode = wrongCategoryException.StatusCode;
                    await context.Response.WriteAsync(wrongCategoryException.Message);
                    break;
                
                default:
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("что то пошло не так");
                    break;
            }
        }
    }
}