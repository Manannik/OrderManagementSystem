using System.ComponentModel.DataAnnotations;

namespace Notification.Infrastructure.Telegram.Models;

public enum OrderStatusModel
{
    [Display(Name = "На складе")]
    InStock,

    [Display(Name = "Передан в доставку")]
    TransferredToDelivery,

    [Display(Name = "В доставке")]
    InDelivery,

    [Display(Name = "Завершен")]
    Completed
}