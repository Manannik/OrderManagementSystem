using OrderProcessingService.Domain.Enums;
using OrderProcessingService.Domain.Extensions;

namespace OrderProcessingService.Domain.Exceptions;

public class ProcessingOrderStageException (string id, Stage stage) : OrderProcessingException(
    $"Заказ {id} находится в статусе {stage.GetDisplayName()}", 409);