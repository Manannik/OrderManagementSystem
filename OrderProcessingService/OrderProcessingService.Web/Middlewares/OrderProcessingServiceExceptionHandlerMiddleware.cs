using OrderProcessingService.Domain.Exceptions;
using Serilog;

namespace OrderProcessingService.Web.Middlewares;

public class OrderProcessingServiceExceptionHandlerMiddleware
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
                    Log.Information("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(statusException.Message);
                    break;
                case InvalidOrderStageException stageException:
                    Log.Information("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync(stageException.Message);
                    break;
                case OrderProcessingDoesNotExistsException orderProcessingDoesNotExistsException:
                    Log.Information("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = orderProcessingDoesNotExistsException.StatusCode;
                    await context.Response.WriteAsync(orderProcessingDoesNotExistsException.Message);
                    break;
                case ProcessingOrderStatusException processingOrderStatusException:
                    Log.Information("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = processingOrderStatusException.StatusCode;
                    await context.Response.WriteAsync(processingOrderStatusException.Message);
                    break;
                case ProcessingOrderStageException processingOrderStageException:
                    Log.Information("Попытка создать заказ с пустым списком Products");
                    context.Response.StatusCode = processingOrderStageException.StatusCode;
                    await context.Response.WriteAsync(processingOrderStageException.Message);
                    break;
                case DeliverOrderException deliverOrderException:
                    Log.Information("Попытка передать заказ в отдел доставки с неправильным статусом"+
                                    $"OrderId: {deliverOrderException.OrderId}, " +
                                    $"Status: {deliverOrderException.Status}, " +
                                    $"Stage: {deliverOrderException.Stage}");
                    context.Response.StatusCode = deliverOrderException.StatusCode;
                    await context.Response.WriteAsync(deliverOrderException.Message);
                    break;
                default:
                    context.Response.StatusCode = 500;
                    Log.Information("что то пошло не так");
                    await context.Response.WriteAsync("что то пошло не так");
                    break;
            }
        }
    }
}