using System.Text;
using Application.BusinessLogic.Models;
using Application.Models;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace IntegrationsTests;

public class ProductControllerTests : IDisposable
{
    private WebApplicationFactory<Program> _webHost;
    private CatalogDbContext _context;
    private ProductRepository _productRepository;
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

                    services.AddDbContext<CatalogDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryCatalogDB");
                    });
                });
            });

        using var scope = _webHost.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        _productRepository = new ProductRepository(_context);
    }

    [Test]
    public async Task CreateProductController_ReturnsSuccess()
    {
        // Arrange
        var fixture = new Fixture();
        // fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        // fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<CreateProductRequest>(f =>
            f.With(request => request.CategoryModelDtos, fixture.CreateMany<CategoryModelDto>(2).ToList())
        );
        var product = fixture.Create<CreateProductRequest>();

        fixture.Customize<List<Category>>(f =>
            f.With(g => g.Select(q=>q.Id), product.CategoryModelDtos.Select(h => h.Id).ToList())
        );
        
        var client = _webHost.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/catalog", content);

        // Assert
        response.EnsureSuccessStatusCode();
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