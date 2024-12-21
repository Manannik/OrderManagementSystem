using System.Net.Http.Json;
using System.Text;
using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Commands.DeleteProduct;
using Application.BusinessLogic.Commands.UpdateProduct;
using Application.BusinessLogic.Models;
using Application.BusinessLogic.Queries;
using Application.Models;
using AutoFixture;
using Confluent.Kafka;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using OrderManagementSystem.Infrastructure;

namespace IntegrationsTests;

public class ProductControllerTests : IDisposable
{
    private WebApplicationFactory<Program> _webHost;
    private HttpClient _client;

    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();

    [SetUp]
    public void SetupForController()
    {
        _webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbDescriptor = services
                        .SingleOrDefault(f => f.ServiceType == typeof(CatalogDbContext));
                    services.Remove(dbDescriptor!);

                    services.AddScoped(_ => _productRepositoryMock.Object);
                    services.AddScoped(_ => _categoryRepositoryMock.Object);
                    services.AddScoped(_ => _mediatorMock.Object);

                    services.AddDbContext<CatalogDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryCatalogDB");
                    });
                });
            });

        _client = _webHost.CreateClient();
    }

    [Test]
    public async Task CreateProduct_ReturnsCreatedResponse()
    {
        var fixture = new Fixture();
        var productRequest = fixture.Create<CreateProductRequest>();

        _mediatorMock
            .Setup(m => m.Send(It.Is<CreateProductCommand>(cmd =>
                    cmd.Name == productRequest.Name &&
                    cmd.Description == productRequest.Description &&
                    cmd.Price == productRequest.Price &&
                    cmd.Quantity == productRequest.Quantity &&
                    cmd.CategoryModelDtos.SequenceEqual(productRequest.CategoryModelDtos)),
                CancellationToken.None))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var response = await _client.PostAsJsonAsync("/catalog", productRequest);
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

        response.EnsureSuccessStatusCode();

        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetById_ReturnsProduct_WhenProductExists()
    {
        var fixture = new Fixture();
        var productId = Guid.NewGuid();
        var productModelDto = fixture.Build<ProductModelDto>().With(p => p.Id, productId)
            .Create();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProductQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productModelDto)
            .Verifiable();

        var response = await _client.GetAsync($"/catalog/{productId}");

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        response.EnsureSuccessStatusCode();

        _mediatorMock.Verify(m => m.Send(It.IsAny<GetProductQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.IsNotEmpty(responseContent);

        var returnedProduct = JsonConvert.DeserializeObject<ProductModelDto>(responseContent);
        Assert.That(returnedProduct, Is.Not.Null);
        Assert.That(returnedProduct.Id, Is.EqualTo(productId));
    }

    [Test]
    public async Task UpdateProduct_ReturnsUpdatedProduct_WhenProductExists()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var product = fixture.Create<Product>();

        var productRequest = new UpdateProductRequest()
        {
            Name = product.Name,
            Description = product.Description,
            CategoryModelDtos = product.Categories.Select(f => new CategoryModelDto()
            {
                Id = f.Id
            }).ToList(),
            Price = product.Price,
            Quantity = product.Quantity
        };

        var productModelDto = new ProductModelDto()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            CategoriesModelDtos = product.Categories.Select(f => new CategoryModelDto()
            {
                Id = f.Id
            }).ToList(),
            Price = product.Price,
            Quantity = product.Quantity,
            CreatedDateUtc = product.CreatedDateUtc,
            UpdatedDateUtc = product.UpdatedDateUtc
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(productModelDto)
            .Verifiable();

        var response = await _client.PostAsJsonAsync($"/catalog/{product.Id}", productRequest);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        response.EnsureSuccessStatusCode();

        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.IsNotEmpty(responseContent);

        var returnedProduct = JsonConvert.DeserializeObject<ProductModelDto>(responseContent);
        returnedProduct.Should().BeEquivalentTo(productModelDto);
    }

    [Test]
    public async Task UpdateProduct_ReturnsFailure_WhenProductDoesNotExist()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var product = fixture.Create<Product>();

        var productRequest = new UpdateProductRequest()
        {
            Name = product.Name,
            Description = product.Description,
            CategoryModelDtos = product.Categories.Select(f => new CategoryModelDto()
            {
                Id = f.Id
            }).ToList(),
            Price = product.Price,
            Quantity = product.Quantity
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ProductDoesNotExistException(product.Id))
            .Verifiable();
        
        var content = new StringContent(JsonConvert.SerializeObject(productRequest), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"/catalog/{product.Id}", content);
        Assert.That((int)response.StatusCode, Is.EqualTo(404));
        _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    
    [Test]
    public async Task Delete_ReturnsDeletedResponse_WhenProductExists()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var productId = Guid.NewGuid();
        var product = fixture.Build<Product>().With(f=>f.Id,productId).Create();
        
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        var response = await _client.DeleteAsync($"/catalog/{product.Id}",CancellationToken.None);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        response.EnsureSuccessStatusCode();
        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TearDown]
    public void TearDown()
    {
        _webHost.Dispose();
    }

    public void Dispose()
    {
        TearDown();
    }
}