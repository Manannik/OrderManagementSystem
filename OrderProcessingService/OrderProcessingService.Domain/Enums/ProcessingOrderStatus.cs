using System.ComponentModel.DataAnnotations;

namespace OrderProcessingService.Domain.Enums;

public enum ProcessingOrderStatus
{
    [Display(Name = "Новый")]
    New,
    [Display(Name = "В обработке")]
    Processing,
    [Display(Name = "Выполнен")]
    Completed
}