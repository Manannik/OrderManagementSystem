using Application.BusinessLogic.Commands.UpdateProduct;
using Application.BusinessLogic.Models;
using Application.Models;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
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

        var existingCategories = new List<Category>()
        {
            new Category()
            {
                Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
            }
        };

        var existingProduct = new Product()
        {
            Id = Guid.Parse("29ada098-6fe2-4c4c-ba6e-1a9788abd04b"),
            Name = "Одежда1",
            Description = "Одежда1",
            Categories = new List<Category>()
            {
                new Category()
                {
                    Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651"),
                    Name = "Одежда"
                }
            },
            Price = 800,
            Quantity = 10,
            CreatedDateUtc = DateTime.UtcNow
        };

        var command = new UpdateProductCommand()
        {
            Request = new UpdateProductRequest()
            {
                Name = "Одежда1",
                Description = "Одежда1",
                Category = new List<CategoryModelDto>()
                {
                    new CategoryModelDto()
                    {
                        Id = Guid.Parse("6af8acea-bfa5-438d-ac76-2767b6f2d651")
                    }
                },
                Price = 777,
                Quantity = 4
            }
        };

        var categoriesRequest = command.Request.Category
            .Select(f=>f.Id).ToList();
        
        _productRepositoryMock.Setup(f => f.GetByIdAsync(command.Id, default))
            .ReturnsAsync(existingProduct);
        
        _categoryRepositoryMock.Setup(f => f.GetByIdAsync(categoriesRequest, default))
            .ReturnsAsync(existingCategories);

        var handler = new UpdateProductCommandHandle(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);

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
            CategoriesModelDtos = updatedProductModelDto.CategoriesModelDtos,
            Price = command.Request.Price
        };

        Assert.Equivalent(mappedCommand, updatedProductModelDto);
    }
}