using FluentValidation;
using Order.Application.Models;

namespace Order.Web.Controllers.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleForEach(f => f.ProductItemModels)
                .ChildRules(productItem =>
                {
                    productItem.RuleFor(item => item.Id)
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