using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Commands.UpdateProduct;
using Application.BusinessLogic.Models;
using Application.Models;
using AutoFixture;
using Domain.Abstractions;
using Domain.Entities;
using Moq;

namespace Tests;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();

    [Fact]
    public async Task Handle_Should_ReturnSuccessResult_WhenUpdateProduct()
    {
        //Arrange
        var fixture = new Fixture();
        var guid = fixture.Create<Guid>();

        var categoriesModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        
        var updateProductRequest = fixture.Build<UpdateProductRequest>()
            .With(f => f.CategoryModelDtos, categoriesModelDto)
            .Create();
        
        var updateProductCommand = fixture.Build<UpdateProductCommand>()
            .With(f => f.Id, guid)
            .With(f => f.Request, updateProductRequest)
            .Create();
        
        var guids = categoriesModelDto.Select(f => f.Id).ToList();
        
        var categories = new List<Category>();
        foreach (var modelDto in categoriesModelDto)
        {
            var category = fixture.Build<Category>()
                .With(f => f.Id, modelDto.Id)
                .Without(x => x.Products)
                .Create();
            categories.Add(category);
        }

        var product = fixture.Build<Product>()
            .With(f => f.Id, guid)
            .With(f=>f.Categories,categories)
            .Create();

        _productRepositoryMock.Setup(f => f.GetByIdAsync(guid, default))
            .ReturnsAsync(product);

        _categoryRepositoryMock.Setup(f => f.GetByIdAsync(guids, default))
            .ReturnsAsync(categories);

        product.Name = updateProductCommand.Request.Name;
        product.Description = updateProductCommand.Request.Description;
        product.Price = updateProductCommand.Request.Price;
        product.Quantity = updateProductCommand.Request.Quantity;
        product.UpdatedDateUtc = DateTime.UtcNow;
        product.Categories = categories;

        _productRepositoryMock.Setup(f => f.UpdateAsync(product, default))
            .Verifiable();
       
        var handler = new UpdateProductCommandHandle(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act

        await handler.Handle(updateProductCommand, default);

        //Assert
        _productRepositoryMock.Verify(f => f.UpdateAsync(It.Is<Product>(g =>
                g.Name == updateProductCommand.Request.Name &&
                g.Description == updateProductCommand.Request.Description &&
                g.Categories.Count == categories.Count &&
                g.Price == updateProductCommand.Request.Price &&
                g.Quantity == updateProductCommand.Request.Quantity &&
                g.UpdatedDateUtc != DateTime.UtcNow
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}