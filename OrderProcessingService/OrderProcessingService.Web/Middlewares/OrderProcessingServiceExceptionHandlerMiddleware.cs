using OrderProcessingService.Domain.Exceptions;

namespace OrderProcessingService.Web.Middlewares;

public class OrderProcessingServiceExceptionHandlerMiddleware(ILogger<OrderProcessingServiceExceptionHandlerMiddleware> _logger)
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
                case InvalidOrderStatusTransitionException statusException:
                    _logger.LogError("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(statusException.Message);
                    break;
                case InvalidOrderStageException stageException:
                    _logger.LogError("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(stageException.Message);
                    break;
                case OrderProcessingDoesNotExistsException orderProcessingDoesNotExistsException:
                    _logger.LogError("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = orderProcessingDoesNotExistsException.StatusCode;
                    await context.Response.WriteAsync(orderProcessingDoesNotExistsException.Message);
                    break;
                case ProcessingOrderStatusException processingOrderStatusException:
                    _logger.LogError("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = processingOrderStatusException.StatusCode;
                    await context.Response.WriteAsync(processingOrderStatusException.Message);
                    break;
                case ProcessingOrderStageException processingOrderStageException:
                    _logger.LogError("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = processingOrderStageException.StatusCode;
                    await context.Response.WriteAsync(processingOrderStageException.Message);
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