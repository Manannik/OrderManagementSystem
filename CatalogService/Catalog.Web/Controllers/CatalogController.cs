using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Commands.DeleteProduct;
using Application.BusinessLogic.Commands.UpdateProduct;
using Application.BusinessLogic.Queries;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers;

[Route("[controller]")]
[ApiController]
public class CatalogController(
    IMediator mediator,
    ILogger<CatalogController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        logger.LogInformation("запуск метод Create, request: {@Request}", request);

         await mediator.Send(new CreateProductCommand()
         {
             Name = request.Name,
             Description = request.Description,
             CategoriesModelDtos = request.CategoryModelDtos,
             Price = request.Price,
             Quantity = request.Quantity
         }, ct);
        
        logger.LogInformation("в результате работы метода Create, продукт успешно создан");
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        logger.LogInformation("запуск метод GetById, выбор продукта по {id}", id);

        var result = await mediator.Send(new GetProductQuery()
        {
            ProductId = id
        }, ct);
        
        logger.LogInformation("продукт с таким {id}, успешно получен", id);
        
        return Ok(result);
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductRequest request,
        CancellationToken ct)
    {
        logger.LogInformation("Вызван метод UpdateProduct с id: {Id} и request: {@Request}", id, request);

        var result = await mediator.Send(new UpdateProductCommand()
        {
            Id = id,
            Request = request
        }, ct);
        
        logger.LogInformation("продукт с таким {id}, успешно обновлен", id);
        
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        logger.LogInformation("Вызван метод Delete с id: {Id}", id);

        await mediator.Send(new DeleteProductCommand()
        {
            Id = id
        }, ct);
        logger.LogInformation("продукт с таким {id}, успешно удален", id);
        
        return Ok();
    }

    [HttpPut("ChangeQuantity/{id:guid}/{newQuantity:int}")]
    public async Task<IActionResult> OrderProductQuantity([FromRoute] Guid id, [FromRoute] int newQuantity,
        CancellationToken ct)
    {
        OrderedQuantity request = new OrderedQuantity()
        {
            Id = id,
            NewQuantity = newQuantity
        };
        
        logger.LogInformation("Вызван метод UpdateQuantity с id: {Id} и новым количеством товара {newQuantity}", id,
            request.NewQuantity);
        
        var productModel = await mediator.Send(new UpdateQuantityCommand()
        {
            Request = request
        }, ct);
        
        logger.LogInformation("Для продукт с {id}, успешно обновлено количество товара", id);
        
        return Ok(productModel);
    }
}