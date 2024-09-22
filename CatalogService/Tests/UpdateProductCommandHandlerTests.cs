using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Commands.UpdateProduct;
using Application.BusinessLogic.Models;
using Application.Models;
using Domain.Abstractions;
using Domain.Entities;
using Moq;

namespace Tests;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;

    public UpdateProductCommandHandlerTests()
    {
        _productRepositoryMock = new();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenProductUpdated()
    {
        //Arrange
        var command = new UpdateProductCommand()
        {
            Request = new UpdateProductRequest()
            {
                Name = "Одежда1",
                Description = "Одежда1",
                Category = new CategoryModelDto()
                {
                    Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
                },
                Price = 777,
                Quantity = 4
            }
        };

        var existingProduct = new Product()
        {
            Id =Guid.Parse("29ada098-6fe2-4c4c-ba6e-1a9788abd04b"),
            Name = "Одежда1",
            Description ="Одежда1",
            Categories = new List<Category>()
            {
                new Category()
                {
                    Id =Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651"),
                    Name = "Одежда"
                }
            },
            Price = 800,
            Quantity = 10,
            CreatedDateUtc = DateTime.UtcNow
        };
        
        _productRepositoryMock.Setup(f => f.GetByIdAsync(command.Id,default))
            .ReturnsAsync(existingProduct);
        
        var handler = new UpdateProductCommandHandle(
            _productRepositoryMock.Object);
        
        //Act
        var updatedProductModelDto = await handler.Handle(command, default);

        //Assert

        var mappedCommand = new ProductModelDto()
        {
            Id = existingProduct.Id,
            Name = command.Request.Name,
            Description = command.Request.Description,
            CreatedDateUtc = existingProduct.CreatedDateUtc,
            UpdatedDateUtc = updatedProductModelDto.UpdatedDateUtc,
            Quantity = command.Request.Quantity,
            CategoriesModelDtos = new()
            {
                new()
                {
                    Id = command.Request.Category.Id
                }
            },
            Price = command.Request.Price
        };
        
        Assert.Equal(mappedCommand,updatedProductModelDto);
    }
}