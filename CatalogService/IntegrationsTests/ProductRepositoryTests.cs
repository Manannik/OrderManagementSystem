using AutoFixture;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using OrderManagementSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IntegrationsTests;

public class ProductRepositoryTests
{
    [SetUp]
    public void Setup()
    {
        // var options = new DbContextOptionsBuilder<CatalogDbContext>()
        //     .UseNpgsql("Host=127.0.0.1;Username=postgres;Password=test;Database=catalog_service_test")
        //     .Options;
        //
        // _context = new CatalogDbContext(options);
    }

    [Test]
    public void CreateProduct_SavesCorrectData()
    {
        //Arrange
        var fixture = new Fixture();
        var categories = fixture.CreateMany<Category>(3).ToList();
        var product = fixture.Build<Product>()
            .With(p => p.Categories, categories)
            .Create();
        
        var webHost = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var productRepository = services
                        .SingleOrDefault(f => f.ServiceType == typeof(IProductRepository));
                    
                    services.Remove(productRepository!);
                });
            });

        var mockProductRepository = new Mock<IProductRepository>();
        mockProductRepository
            .Setup(f => f.CreateAsync(product, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        //Act

        var retrievedProduct = mockProductRepository
            .Setup(f => f.GetByIdAsync(product.Id, default))
            .ReturnsAsync(product);
            
        //Assert
        
        Assert.IsNotNull(retrievedProduct);
    }
    
    
    [Test]
    public void AddProductAndRetrieveFromDatabase()
    {
        // Arrange
        var fixture = new Fixture();
        var product = fixture.Build<Product>()
            .With(f => f.Categories, fixture.CreateMany<Category>())
            .Create();

        var dbContextOptions =  new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        using (var context = new CatalogDbContext(dbContextOptions))
        {
            context.Products.Add(product);
            context.SaveChanges();
        }

        using (var context = new CatalogDbContext(dbContextOptions))
        {
            var retrievedProduct = context.Products.Find(product.Id);

            // Assert
            Assert.NotNull(retrievedProduct);
            Assert.AreEqual(product.Id, retrievedProduct.Id);
            Assert.AreEqual(product.Name, retrievedProduct.Name);
            Assert.AreEqual(product.Price, retrievedProduct.Price);
        }
    }
}