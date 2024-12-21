using AutoFixture;
using Confluent.Kafka;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace IntegrationsTests;

public class ProductRepositoryTests : IDisposable
{
    private WebApplicationFactory<Program> _webHost;
    private ProductRepository _productRepository;
    private IServiceScope _serviceScope;
    
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
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());  
                    });
                });
            });

        _serviceScope = _webHost.Services.CreateScope();
        var context = _serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        _productRepository = new ProductRepository(context);
        
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        context.SaveChanges();
    }

    [Test]
    public async Task CreateAsync_SavesCorrectData()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));

        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var product = fixture.Create<Product>();

        // Act
        await _productRepository.CreateAsync(product, CancellationToken.None);

        // Assert
        var savedProduct = await _productRepository.GetByIdAsync(product.Id, CancellationToken.None);
        Assert.NotNull(savedProduct);
        Assert.That(savedProduct.Name, Is.EqualTo(product.Name));
    }

    [Test]
    public async Task GetByIdAsync_WhenProductExists_ReturnsExpectedProduct()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));

        var category1 = fixture.Create<Category>();
        var category2 = fixture.Create<Category>();

        var expectedProduct = fixture.Build<Product>()
            .With(p => p.Categories, new List<Category> { category1, category2 })
            .Create();

        var context = _serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await context.Categories.AddRangeAsync(expectedProduct.Categories);
        await context.Products.AddAsync(expectedProduct);
        await context.SaveChangesAsync();

        // Act
        var actualProduct = await _productRepository.GetByIdAsync(expectedProduct.Id, default);

        // Assert
        Assert.That(actualProduct, Is.Not.Null);
        Assert.That(actualProduct.Id, Is.EqualTo(expectedProduct.Id));
        Assert.That(actualProduct.Name, Is.EqualTo(expectedProduct.Name));
        Assert.That(actualProduct.Description, Is.EqualTo(expectedProduct.Description));
        Assert.That(actualProduct.Price, Is.EqualTo(expectedProduct.Price));
        Assert.That(actualProduct.Quantity, Is.EqualTo(expectedProduct.Quantity));
        Assert.That(actualProduct.Categories.Count, Is.EqualTo(expectedProduct.Categories.Count));

        foreach (var expectedCategory in expectedProduct.Categories)
        {
            var actualCategory = actualProduct.Categories.FirstOrDefault(c => c.Id == expectedCategory.Id);
            Assert.That(actualCategory, Is.Not.Null);
            Assert.That(actualCategory.Name, Is.EqualTo(expectedCategory.Name));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenProductDoesNotExists_ReturnsNull()
    {
        // Act
        var actualProduct = await _productRepository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.That(actualProduct, Is.Null);
    }

    [Test]
    public async Task UpdateAsync_WhenProductExists_UpdatesProductDetails()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));

        var category1 = fixture.Create<Category>();
        var category2 = fixture.Create<Category>();

        var product = fixture.Build<Product>()
            .With(p => p.Categories, new List<Category> { category1, category2 })
            .Create();

        var context = _serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        // Act
        product.Name = "NewName";
        product.Description = "NewDescription";
        product.Price = 99.99m;
        product.Quantity = 19;

        await _productRepository.UpdateAsync(product, CancellationToken.None);

        // Assert
        var updatedProduct = await context.Products.FindAsync(product.Id);

        Assert.That(updatedProduct, Is.Not.Null);
        Assert.That(updatedProduct.Name, Is.EqualTo(product.Name));
        Assert.That(updatedProduct.Description, Is.EqualTo(product.Description));
        Assert.That(updatedProduct.Price, Is.EqualTo(product.Price));
        Assert.That(updatedProduct.Quantity, Is.EqualTo(product.Quantity));
    }

    [Test]
    public async Task DeleteAsync_WhenProductExists_DeletesProductFromDatabase()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));

        var category1 = fixture.Create<Category>();
        var category2 = fixture.Create<Category>();

        var product = fixture.Build<Product>()
            .With(p => p.Categories, new List<Category> { category1, category2 })
            .Create();

        var context = _serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        // Act
        await _productRepository.DeleteAsync(product, CancellationToken.None);

        // Assert
        var deletedProduct = await context.Products.FindAsync(product.Id);
        Assert.That(deletedProduct, Is.Null);
    }

    [Test]
    public async Task ExistAsync_WhenProductWithNameExists_ReturnsTrue()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));

        var category1 = fixture.Create<Category>();
        var category2 = fixture.Create<Category>();

        var product = fixture.Build<Product>()
            .With(p => p.Categories, new List<Category> { category1, category2 })
            .Create();

        var context = _serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        // Act
        var result = await _productRepository.ExistAsync(product.Name, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True);
    }

    [TearDown]
    public void TearDown()
    {
        _serviceScope?.Dispose();
        _webHost.Dispose(); 
    }

    public void Dispose()
    {
        TearDown();
    }
}
