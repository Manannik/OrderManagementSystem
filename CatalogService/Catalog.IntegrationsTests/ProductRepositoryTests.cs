using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace IntegrationsTests;

public class ProductRepositoryTests : IDisposable
{
    private WebApplicationFactory<Program> _webHost;

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
    }

    [Test]
    public async Task CreateProduct_SavesCorrectData()
    {
        // Arrange
        using var scope = _webHost.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var productRepository = new ProductRepository(context);

        var fixture = new Fixture();
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));
        
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var product = fixture.Create<Product>();

        // Act
        await productRepository.CreateAsync(product, default);

        // Assert
        var savedProduct = await context.Products.FindAsync(product.Id);
        Assert.NotNull(savedProduct);
        Assert.That(savedProduct.Name, Is.EqualTo(product.Name));
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