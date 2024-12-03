using Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace IntegrationsTests;

public class ProductControllerEndToEndTests  : IDisposable
{
    private WebApplicationFactory<Program> _webHost;
    private IServiceScope _serviceScope;
    private HttpClient _client;
    private IProductRepository _productRepository;
    private ICategoryRepository _categoryRepository;
    private IMediator _mediator;

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
                    
                    services.AddScoped<IProductRepository, ProductRepository>();
                    services.AddScoped<ICategoryRepository, CategoryRepository>();
                    services.AddDbContext<CatalogDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryCatalogDB");
                    });
                    
                });
            });
        
        _client = _webHost.CreateClient();
        _serviceScope = _webHost.Services.CreateScope();
        
        var context = _serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        _productRepository = _serviceScope.ServiceProvider.GetRequiredService<IProductRepository>();
        _categoryRepository = _serviceScope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        _mediator = _serviceScope.ServiceProvider.GetRequiredService<IMediator>();
        
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        context.SaveChanges();
    }
    
    [Test]
    public async Task CreateProduct_ReturnsCreatedResponse()
    {
        return;
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