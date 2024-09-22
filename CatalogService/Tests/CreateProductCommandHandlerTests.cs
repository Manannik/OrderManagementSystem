using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Moq;

namespace Tests;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;

    public CreateProductCommandHandlerTests()
    {
        _productRepositoryMock = new();
        _categoryRepositoryMock = new();
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenProductExist()
    {
        //Arrange
        var command = new CreateProductCommand()
        {
            Name = "Одежда1",
            Description = "Одежда1",
            CategoriesModelDtos = new List<CategoryModelDto>()
            {
                new CategoryModelDto()
                {
                    Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
                }
            },
            Price = 777,
            Quantity = 4
        };

        var product = new Product()
        {
            Name = command.Name,
            Description = command.Description,
            Categories = new List<Category>()
            {
                new Category()
                {
                    Id =Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651"),
                    Name = "Одежда"
                }
            },
            Price = command.Price,
            Quantity = command.Quantity,
            CreatedDateUtc = DateTime.UtcNow
        };

        _productRepositoryMock.Setup(f => f.ExistAsync(command.Name,default)).ReturnsAsync(true);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(command, default);

        //Assert
        await Assert.ThrowsAsync<ProductAlreadyExistException>(act);
    }
    
    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenProductDoesNotExist()
    {
        //Arrange
        var command = new CreateProductCommand()
        {
            Name = "Одежда1",
            Description = "Одежда1",
            CategoriesModelDtos = new List<CategoryModelDto>()
            {
                new CategoryModelDto()
                {
                    Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
                }
            },
            Price = 777,
            Quantity = 4
        };

        var product = new Product()
        {
            Name = "Одежда2",
            Description ="Одежда2",
            Categories = new List<Category>()
            {
                new Category()
                {
                    Id =Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651"),
                    Name = "Одежда"
                }
            },
            Price = 100,
            Quantity = 100,
            CreatedDateUtc = DateTime.UtcNow
        };

        _productRepositoryMock.Setup(f => f.ExistAsync(command.Name,default)).ReturnsAsync(false);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(command, default);

        //Assert
    }
}