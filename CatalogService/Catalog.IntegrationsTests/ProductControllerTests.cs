using System.Net.Http.Json;
using Application.BusinessLogic.Commands.CreateProduct;
using Application.Models;
using AutoFixture;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
            .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var response = await _client.PostAsJsonAsync("/catalog", productRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Request failed with status {response.StatusCode}. Response content: {errorContent}");
        }

        response.EnsureSuccessStatusCode();

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));

        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.IsEmpty(responseContent);
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