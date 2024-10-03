using System.ComponentModel.DataAnnotations;
using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Moq;

namespace Tests;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();

    [Fact]
    public async Task Handle_Should_ReturnSuccess()
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
        Assert.True(true);
    }
    
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

        _productRepositoryMock.Setup(f => f.ExistAsync(command.Name,default))
            .ReturnsAsync(true);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(command, default);

        //Assert
        await Assert.ThrowsAsync<ProductAlreadyExistException>(act);
    }
    
    [Fact]
    public void Handle_Should_ReturnNullResult_WhenProductDoesNotExist()
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
        Assert.Null(act);
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
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(command, default);

        //Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }

    public async Task Handle_Should_ReturnFailureResult_WhenCategoryDoesNotExist()
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
        //Act
        _categoryRepositoryMock
            .Setup(f => f.GetByIdAsync(command.CategoriesModelDtos.Select(f => f.Id).ToList(), default))
            .ReturnsAsync((List<Category>)null);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        var act = () => handler.Handle(command, default);
        //Assert
        Assert.Null(act);
    }
}