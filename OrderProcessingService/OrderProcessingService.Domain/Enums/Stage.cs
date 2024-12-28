using System.ComponentModel.DataAnnotations;

namespace OrderProcessingService.Domain.Enums;

public enum Stage
{
    [Display(Name = "Сборка")]
    Assembly,

    [Display(Name = "Доставка")]
    Delivery
}