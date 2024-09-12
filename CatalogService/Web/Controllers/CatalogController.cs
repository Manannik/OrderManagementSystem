using Application.BusinessLogic.Commands.CreateProduct;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("[controller]")]
public class CatalogController(IMediator mediator) : ControllerBase
{
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        //var result = await mediator.Send(new CreateProductCommand(request), ct);
        await mediator.Send(new CreateProductCommand(request.Name, request.Description, request.Category,
            request.Price, request.Quantity),ct);
        return Ok();
    }
    
    [HttpGet("{id}")]
    public Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        return null;
    }
    
    [HttpPost("Update")]
    public Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        return null;
    }
    
    [HttpDelete("{id}")]
    public Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        return null;
    }
    
    [HttpPut]
    public Task<IActionResult> UpdateQuantity(CancellationToken ct)
    {
        return null;
    }
}