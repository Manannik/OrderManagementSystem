using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Commands.UpdateProduct;
using Application.BusinessLogic.Models;
using Application.Models;
using Domain.Abstractions;
using Moq;

namespace Tests;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;

    public UpdateProductCommandHandlerTests()
    {
        _productRepositoryMock = new();
        _categoryRepositoryMock = new();
    }
    
    [Fact]
    public Task Handle_Should_ReturnFailureResult_WhenProductExist()
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

        var handler = new UpdateProductCommandHandle(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
        
        //Act
        var act = () => handler.Handle(command, default);

        //Assert
        
        
    }
}