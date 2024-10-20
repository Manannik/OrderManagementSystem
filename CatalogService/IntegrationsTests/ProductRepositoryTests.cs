using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace IntegrationsTests;

public class ProductRepositoryTests : IDisposable
{
    private WebApplicationFactory<Program> _webHost;
    private CatalogDbContext _context;
    private ProductRepository _productRepository;

    [SetUp]
    public void Setup()
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
}
