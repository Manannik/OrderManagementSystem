using FluentValidation;
using Order.Application.Abstractions;
using Order.Application.Models;

namespace Order.Web.Controllers.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        private readonly IQuantityService _quantityService;
        
        public CreateOrderRequestValidator(IQuantityService quantityService)
        {
            _quantityService = quantityService;
            
            RuleForEach(f => f.ProductItemModels)
                .ChildRules(productItem =>
                {
                    productItem.RuleFor(item => item.ProductId)
                        .NotEqual(Guid.Empty).WithMessage("ID продукта не должен быть пустым GUID.");

                    productItem.RuleFor(item => item.Quantity)
                        .GreaterThan(0).WithMessage("Количество заказываемого товара должно быть больше 0");
                });
            
            RuleFor(request => request.ProductItemModels)
                .Must(productItemModels => productItemModels != null && productItemModels.Any())
                .WithMessage("Товары не должны быть Null или пустыми");
        }
    }
}