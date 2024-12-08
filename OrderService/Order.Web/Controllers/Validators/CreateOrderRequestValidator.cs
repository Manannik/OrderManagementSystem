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
                    productItem.RuleFor(item => item.Id)
                        .NotEqual(Guid.Empty).WithMessage("ID продукта не должен быть пустым GUID.");

                    productItem.RuleFor(item => item.Quantity)
                        .GreaterThan(0).WithMessage("Количество заказываемого товара должно быть больше 0");
                });
            
            RuleFor(request => request.ProductItemModels)
                .Must(productItemModels => productItemModels != null && productItemModels.Any())
                .WithMessage("Товары не должны быть Null или пустыми");
            
            RuleFor(request => request)
                .MustAsync(ValidateQuantitiesAsync)
                .WithMessage("Некоторые товары имеют недоступное количество.");
        }
        
        private async Task<bool> ValidateQuantitiesAsync(CreateOrderRequest request, CancellationToken ct)
        {
            var productItemModels = request.ProductItemModels.ToList();
            var result = await _quantityService.TryChangeQuantityAsync(productItemModels, ct);
            return result.IsSuccess;
        }
    }
}