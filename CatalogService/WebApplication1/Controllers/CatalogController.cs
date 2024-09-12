using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Commands.DeleteProduct;
using Application.BusinessLogic.Commands.UpdateProduct;
using Application.BusinessLogic.Queries;
using Application.Models;
using Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Controllers.Validators;

namespace WebApplication1.Controllers;

[Route("[controller]")]
public class CatalogController(IMediator mediator,
    IValidator<CreateProductRequest> createProductRequestValidator,
    IValidator<UpdateProductRequest> updateProductRequestValidator) : ControllerBase
{
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var validationResult = createProductRequestValidator.Validate(request); 
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }
        await mediator.Send(new CreateProductCommand(request.Name, request.Description, request.Category,
            request.Price, request.Quantity), ct);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetProductQuery(id), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductRequest request,
        CancellationToken ct)
    {
        var validationResult = updateProductRequestValidator.Validate(request); 
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }
        var result = await mediator.Send(new UpdateProductCommand(id, request), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteProductCommand(id), ct);
        return Ok();
    }

    [HttpPut("ChangeQuantity/{id:guid}/{newQuantity:int}")]
    public async Task<IActionResult> UpdateQuantity([FromRoute] Guid id,[FromRoute] int newQuantity, CancellationToken ct)
    {
        if (newQuantity < 0)
        {
            throw new QuantityException();
        }
        var productModel = await mediator.Send(new UpdateQuantityCommand(id, newQuantity), ct);
        return Ok(productModel);
    }
}