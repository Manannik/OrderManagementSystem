using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        // Act
        
        // Assert

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