using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Commands.DeleteProduct;
using Application.BusinessLogic.Commands.UpdateProduct;
using Application.BusinessLogic.Queries;
using Application.Models;
using Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("[controller]")]
[ApiController]
public class CatalogController(
    IMediator mediator,
    IValidator<CreateProductRequest> createProductRequestValidator,
    IValidator<UpdateProductRequest> updateProductRequestValidator,
    ILogger<CatalogController> _logger) : ControllerBase
{
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        _logger.LogInformation("запуск метод Create, request: {@Request}", request);

        var validationResult = createProductRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("ошибка валидации при работе метода Create: {@Errors}", validationResult.Errors);
            return BadRequest(validationResult.ToDictionary());
        }

        await mediator.Send(new CreateProductCommand()
        {
            Name = request.Name,
            Description = request.Description,
            CategoriesModelDtos = request.CategoryModelDtos,
            Price = request.Price,
            Quantity = request.Quantity
        }, ct);
        _logger.LogInformation("в результате работы метода Create, продукт успешно создан");
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        _logger.LogInformation("запуск метод GetById, выбор продукта по {id}", id);

        var result = await mediator.Send(new GetProductQuery()
        {
            ProductId = id
        }, ct);
        _logger.LogInformation("продукт с таким {id}, успешно получен", id);
        return Ok(result);
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductRequest request,
        CancellationToken ct)
    {
        _logger.LogInformation("Вызван метод UpdateProduct с id: {Id} и request: {@Request}", id, request);

        var validationResult = updateProductRequestValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("ошибка валидации при работе метода UpdateProduct: {@Errors}", validationResult.Errors);
            return BadRequest(validationResult.ToDictionary());
        }

        var result = await mediator.Send(new UpdateProductCommand()
        {
            Id = id,
            Request = request
        }, ct);
        _logger.LogInformation("продукт с таким {id}, успешно обновлен", id);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Вызван метод Delete с id: {Id}", id);

        await mediator.Send(new DeleteProductCommand()
        {
            Id = id
        }, ct);
        _logger.LogInformation("продукт с таким {id}, успешно удален", id);
        return Ok();
    }

    [HttpPut("ChangeQuantity/{id:guid}/{newQuantity:int}")]
    public async Task<IActionResult> UpdateQuantity([FromRoute] Guid id, [FromRoute] int newQuantity,
        CancellationToken ct)
    {
        _logger.LogInformation("Вызван метод UpdateQuantity с id: {Id} и новым количеством товара {newQuantity}", id,
            newQuantity);

        if (newQuantity < 0)
        {
            _logger.LogInformation("Нельзя задать количество меньше 0");
            throw new QuantityException();
        }

        var productModel = await mediator.Send(new UpdateQuantityCommand()
        {
            Id = id,
            NewQuantity = newQuantity
        }, ct);
        _logger.LogInformation("Для продукт с {id}, успешно обновлено количество товара", id);
        return Ok(productModel);
    }
}