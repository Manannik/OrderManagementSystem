using System.ComponentModel.DataAnnotations;
using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Models;
using Domain.Abstractions;
using Domain.Exceptions;
using Moq;

namespace Tests;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenProductExist()
    {
        //Arrange
        var command = new CreateProductCommand()
        {
            Name = "Одежда1",
            Description = "Одежда1",
            CategoriesModelDtos =
            [
                new CategoryModelDto()
                {
                    Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
                }
            ],
            Price = 777,
            Quantity = 4
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
    public async Task Handle_Should_ReturnFailureResult_WhenProductDoesNotExist()
    {
        //Arrange
        var command = new CreateProductCommand()
        {
            Name = "Одежда1",
            Description = "Одежда1",
            CategoriesModelDtos =
            [
                new CategoryModelDto()
                {
                    Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
                }
            ],
            Price = 777,
            Quantity = 4
        };

        _productRepositoryMock.Setup(f => f.ExistAsync(command.Name,default))
            .ReturnsAsync(false);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(command, default);

        //Assert
        await Assert.ThrowsAsync<ProductAlreadyExistException>(act);
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenProductNameEmpty()
    {
        //Arrange
        var command = new CreateProductCommand()
        {
            Name = "",
            Description = "Одежда1",
            CategoriesModelDtos =
            [
                new CategoryModelDto()
                {
                    Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
                }
            ],
            Price = 777,
            Quantity = 4
        };

        _productRepositoryMock.Setup(f => f.ExistAsync(command.Name,default))
            .ReturnsAsync(false);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(command, default);

        //Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenProductDescriptionEmpty()
    {
        //Arrange
        var command = new CreateProductCommand()
        {
            Name = "Одежда1",
            Description = "",
            CategoriesModelDtos =
            [
                new CategoryModelDto()
                {
                    Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
                }
            ],
            Price = 777,
            Quantity = 4
        };

        _productRepositoryMock.Setup(f => f.ExistAsync(command.Name,default))
            .ReturnsAsync(false);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(command, default);

        //Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }
}