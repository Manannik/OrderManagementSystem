using System.Text;
using AutoFixture;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Newtonsoft.Json;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace IntegrationsTests;

public class ProductRepositoryTests : IDisposable
{
    private WebApplicationFactory<Program> _webHost;
    private CatalogDbContext _context;
    private ProductRepository _productRepository;
    private CategoryRepository _categoryRepository;

    [SetUp]
    public void SetupForRepository()
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
                    
                    var mockProductRepository = new Mock<IProductRepository>();
                    var mockCategoryRepository = new Mock<ICategoryRepository>();

                    services.AddScoped(_=>mockProductRepository.Object);
                    services.AddScoped(_=>mockCategoryRepository.Object);
                });
            });
    }
    
    [Test]
    public async Task CreateProduct_SavesCorrectData()
    {
        // Arrange
        var fixture = new Fixture();
        var product = fixture.Create<Product>();

        // Act
        await _productRepository.CreateAsync(product, default);

        // Assert
        var savedProduct = await _context.Products.FindAsync(product.Id);
        Assert.NotNull(savedProduct);
        Assert.That(savedProduct.Name, Is.EqualTo(product.Name));
    }

    [TearDown]
    public void TearDown()
    {
        _webHost.Dispose();
        _context.Dispose();
    }

    public void Dispose()
    {
        TearDown();
    }

    [Test]
    public async Task CreateProductController_ReturnsSuccess()
    {
        // Arrange
        
        var fixture = new Fixture();
        
        var product = fixture.Create<Product>();
        
        var client = _webHost.CreateClient();
        
        var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
        
        // Act
        
        var response = await client.PostAsync("/api/catalog/create", content);
        
        // Assert
        
        response.EnsureSuccessStatusCode();
    }
}