using FluentValidation;
using Order.Application.Abstractions;
using Order.Application.Models;

namespace Order.Web.Controllers.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator(ICatalogServiceClient catalogServiceClient)
    {
        RuleForEach(f => f.ProductItemModels.Select(f=>f.Id))
            .NotEqual(Guid.Empty).WithMessage("ID продукта не должен быть пустым GUID.");
        
        RuleForEach(request => request.ProductItemModels.Select(f=>f.Id))
            .MustAsync(async (productItemModel, cancellationToken) =>
                await catalogServiceClient.ProductExistsAsync(productItemModel, cancellationToken))
            .WithMessage((request, productItemModel) => 
                $"Продукт с ID {productItemModel} не существует.");

        RuleForEach(request => request.ProductItemModels)
            .ChildRules(productItem =>
            {
                productItem.RuleFor(item => item.Quantity)
                    .GreaterThan(0).WithMessage("Количество заказываемого товара должно быть больше 0");
            });
    }
}