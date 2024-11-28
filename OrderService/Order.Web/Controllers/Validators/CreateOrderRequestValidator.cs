using FluentValidation;
using Order.Application.Abstractions;
using Order.Application.Models;

namespace Order.Web.Controllers.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator(ICatalogServiceClient catalogServiceClient)
        {
            RuleForEach(f => f.ProductItemModels.Select(f => f.Id))
                .NotEqual(Guid.Empty).WithMessage("ID продукта не должен быть пустым GUID.");

            RuleFor(request => request.ProductItemModels)
                .Must(productItemModels => productItemModels != null && productItemModels.Any())
                .WithMessage("Товары не должны быть Null или пустыми");

            RuleForEach(request => request.ProductItemModels)
                .ChildRules(productItem =>
                {
                    productItem.RuleFor(item => item.Quantity)
                        .GreaterThan(0).WithMessage("Количество заказываемого товара должно быть больше 0");
                });
        }
    }
}