using System.ComponentModel.DataAnnotations;
using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Models;
using AutoFixture;
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
        var fixture = new Fixture();
        var categoriesModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        var createProductCommand = fixture.Build<CreateProductCommand>()
            .With(f=>f.CategoriesModelDtos, categoriesModelDto)
            .Create();

        var categoriesId = createProductCommand.CategoriesModelDtos.Select(f => f.Id).ToList();

        var categories = new List<Category>();
        foreach (var modelDto in categoriesModelDto)
        {
            var category = fixture.Build<Category>()
                .With(f => f.Id, modelDto.Id)
                .Create();
            categories.Add(category);
        }

        _categoryRepositoryMock.Setup(f => f.GetByIdAsync(categoriesId, default))
            .ReturnsAsync(categories);
        
        // _categoryRepositoryMock.Setup(f => f.GetByIdAsync(categoriesId,default))
        //     .ReturnsAsync(categoriesId.Select(f =>
        //     {
        //        return fixture.Build<Category>()
        //             .With(g => g.Id, f)
        //             .Create();
        //     }).ToList());
        
        _productRepositoryMock.Setup(f => f.ExistAsync(createProductCommand.Name,default))
            .ReturnsAsync(false);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        await handler.Handle(createProductCommand, default);

        //Assert
        _productRepositoryMock.Verify(f => f.ExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenProductAlreadyExist()
    {
        //Arrange
        var fixture = new Fixture();
        var categoryModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        var createProductCommand = fixture.Build<CreateProductCommand>()
            .With(f=>f.CategoriesModelDtos, categoryModelDto)
            .Create();

        var product = fixture.Build<Product>()
            .With(f => f.Categories, new List<Category>());
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(createProductCommand, default);

        //Assert
        await Assert.ThrowsAsync<ProductAlreadyExistException>(act);
    }
    
    [Fact]
    public void Handle_Should_ReturnNullResult_WhenProductDoesNotExist()
    {
        //Arrange
        var fixture = new Fixture();
        var categoryModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        var createProductCommand = fixture.Build<CreateProductCommand>()
            .With(f=>f.CategoriesModelDtos, categoryModelDto)
            .Create();

        _productRepositoryMock.Setup(f => f.ExistAsync(createProductCommand.Name,default))
            .ReturnsAsync(false);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(createProductCommand, default);

        //Assert
        Assert.ThrowsAsync<ProductDoesNotExistException>(act);
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenProductNameEmpty()
    {
        //Arrange
        var fixture = new Fixture();
        var categoryModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        
        var createProductCommand = fixture.Build<CreateProductCommand>()
            .With(f=>f.CategoriesModelDtos, categoryModelDto)
            .With(f=>f.Name,string.Empty)
            .Create();
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(createProductCommand, default);

        //Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenProductDescriptionEmpty()
    {
        //Arrange
        var fixture = new Fixture();
        var categoryModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        var createProductCommand = fixture.Build<CreateProductCommand>()
            .With(f=>f.CategoriesModelDtos, categoryModelDto)
            .With(f=>f.Description,string.Empty)
            .Create();
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(createProductCommand, default);

        //Assert
        await Assert.ThrowsAsync<ValidationException>(act);
    }
    
    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenCategoryDoesNotExist()
    {
        //Arrange
        var fixture = new Fixture();
        var categoryModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        var createProductCommand = fixture.Build<CreateProductCommand>()
            .With(f=>f.CategoriesModelDtos, categoryModelDto)
            .Create();
        
        //Act
        _categoryRepositoryMock
            .Setup(f => f.GetByIdAsync(categoryModelDto.Select(g=>g.Id).ToList(), default))
            .ReturnsAsync((List<Category>)null);
        
        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        var act = () => handler.Handle(createProductCommand, default);
        
        //Assert
        await Assert.ThrowsAsync<WrongCategoryException>(act);
    }
}