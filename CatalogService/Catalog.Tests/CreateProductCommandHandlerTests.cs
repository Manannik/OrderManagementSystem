using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Models;
using Application.Models;
using AutoFixture;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using Moq;
using WebApplication.Controllers.Validators;

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
            .With(f => f.CategoryModelDtos, categoriesModelDto)
            .Create();

        var categoriesId = createProductCommand.CategoryModelDtos.Select(f => f.Id).ToList();

        var categories = new List<Category>();
        foreach (var modelDto in categoriesModelDto)
        {
            var category = fixture.Build<Category>()
                .With(f => f.Id, modelDto.Id)
                .Without(x => x.Products)
                .Create();
            categories.Add(category);
        }

        _categoryRepositoryMock.Setup(f => f.GetByIdAsync(categoriesId, default))
            .ReturnsAsync(categories);

        _productRepositoryMock.Setup(f => f.ExistAsync(createProductCommand.Name, default))
            .ReturnsAsync(false); 

        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);

        //Act
        await handler.Handle(createProductCommand, default);

        //Assert
        _productRepositoryMock.Setup(f =>
            f.ExistAsync(
                It.Is<string>(f =>
                    f == createProductCommand.Name),
                It.IsAny<CancellationToken>()));

        _productRepositoryMock.Verify(f => f.CreateAsync(It.Is<Product>(g =>
            g.Name == createProductCommand.Name &&
            g.Description == createProductCommand.Description &&
            g.Categories == categories &&
            g.Price == createProductCommand.Price &&
            g.Quantity == createProductCommand.Quantity
        ), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenProductAlreadyExist()
    {
        //Arrange
        var fixture = new Fixture();
        var categoryModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        var createProductCommand = fixture.Build<CreateProductCommand>()
            .With(f => f.CategoryModelDtos, categoryModelDto)
            .Create();
        
        _productRepositoryMock.Setup(f => f.ExistAsync(createProductCommand.Name, default))
            .ReturnsAsync(true);

        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(createProductCommand, default);

        //Assert
        await Assert.ThrowsAsync<ProductAlreadyExistException>(act);
    }

    [Theory]
    [InlineData("Name", "'Name' должно быть заполнено.")]
    [InlineData("Description", "'Description' должно быть заполнено.")]
    public async Task Validator_Should_ReturnFailureResult_WhenProductPropertyIsEmpty(string propertyName, 
        string expectedErrorMessage)
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Build<CreateProductRequest>()
            .With(f => f.Name, "Some Name") 
            .With(f => f.Description, "Some Description")
            .Create();

        if (propertyName == "Name")
        {
            request.Name = null;
        }
        else if (propertyName == "Description")
        {
            request.Description = null;
        }

        var validator = new CreateProductRequestValidator();

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(expectedErrorMessage, result.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_WhenCategoryDoesNotExist()
    {
        //Arrange
        var fixture = new Fixture();
        var categoryModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        var createProductCommand = fixture.Build<CreateProductCommand>()
            .With(f => f.CategoryModelDtos, categoryModelDto)
            .Create();

        var categoriesId = createProductCommand.CategoryModelDtos.Select(f => f.Id).ToList();
        
        //Act
        _categoryRepositoryMock.Setup(f => f.GetByIdAsync(categoriesId, default))
            .ReturnsAsync(new List<Category>());

        var handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);

        var act = () => handler.Handle(createProductCommand, default);

        //Assert
        await Assert.ThrowsAsync<WrongCategoryException>(act);
    }
}