using System.ComponentModel.DataAnnotations;

namespace Messaging.Kafka.Models;

public enum StageModel
{
    [Display(Name = "Сборка")]
    Assembly,

    [Display(Name = "Доставка")]
    Delivery,
    
    [Display(Name = "Выполнен")]
    Completed
}