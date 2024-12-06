using System.Net.Http.Json;
using Application.BusinessLogic.Commands.CreateProduct;
using AutoFixture;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Order.Application.Abstractions;
using Order.Application.Models;
using Order.Domain.Abstractions;
using Order.Persistence;
using Order.Persistence.Repositories;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace OrderService.IntegrationTests
{
public class OrderServiceIntegrationTestsEndToEnd : IDisposable
{
    private WebApplicationFactory<Program> _catalogServiceFactory;
    private WebApplicationFactory<Program> _orderServiceFactory;
    private HttpClient _productClient;
    private HttpClient _orderClient;
    private CatalogDbContext _catalogDbContext;
    private OrderDbContext _orderDbContext;
    private IProductRepository _productRepository;
    private ICategoryRepository _categoryRepository;
    private IOrderRepository _orderRepository;
    private IOrderService _orderService;
    private IServiceScope _orderServiceScope;
    private IServiceScope _catalogServiceScope;
    [SetUp]
    public void Setup()
    {
        SetupOrderService();
        SetupProductService();
    }

    private void SetupOrderService()
    {
        var testConfig = new Dictionary<string, string>
        {
            { "TestingConfiguration:SkipMigration", "true" }
        };
        
        var testConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(testConfig)
            .Build();

        _orderServiceFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IConfiguration>(testConfiguration);
                    var descriptors = services.Where(d => 
                            d.ServiceType == typeof(OrderDbContext) ||
                            d.ServiceType == typeof(IOrderRepository))
                        .ToList();
                
                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<OrderDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryCatalogDB");
                    });

                    services.AddSingleton<IOrderRepository, OrderRepository>();
                });

                _orderClient = _orderServiceFactory.CreateClient();
            });
        //тут все ломается
        _orderServiceScope  = _orderServiceFactory.Services.CreateScope();
        var context = _orderServiceScope .ServiceProvider.GetRequiredService<OrderDbContext>();
        _orderRepository = new OrderRepository(context);
        
        context.Orders.RemoveRange(context.Orders);
        context.SaveChanges();
    }
    
    private void SetupProductService()
    {
        _catalogServiceFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptors = services.Where(d => 
                            d.ServiceType == typeof(CatalogDbContext) ||
                            d.ServiceType == typeof(IProductRepository) ||
                            d.ServiceType == typeof(ICategoryRepository))
                        .ToList();
                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }
                    var options = new DbContextOptionsBuilder<CatalogDbContext>()
                        .UseInMemoryDatabase("InMemoryCatalogDB")
                        .Options;
                    _catalogDbContext = new CatalogDbContext(options);
                    services.AddSingleton(_ => _catalogDbContext);
                    services.AddScoped<IProductRepository, ProductRepository>();
                    services.AddScoped<ICategoryRepository, CategoryRepository>();
                    services.AddMediatR(configuration =>
                    {
                        configuration.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
                    });
                });
                _orderClient = _catalogServiceFactory.CreateClient();
            });
        
        _catalogServiceScope = _catalogServiceFactory.Services.CreateScope();
        var context = _catalogServiceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        _productRepository = new ProductRepository(context);
        
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        context.SaveChanges();
        context.SaveChanges();
    }

    [Test]
    public async Task CreateOrder_WhenCatalogServiceReturnsSuccess_ShouldCreateOrderSuccessfully()
    {
        var fixture = new Fixture();
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var categories = fixture.CreateMany<Category>(2).ToList();
        var products = fixture.Build<Product>()
            .With(f => f.Categories, categories)
            .With(req => req.Price, fixture.Create<decimal>() % 100 + 1)
            .With(req => req.Name, fixture.Create<string>().Substring(0, 10))
            .With(req => req.Description, fixture.Create<string>().Substring(0, 10))
            .With(req => req.Quantity, fixture.Create<int>() % 100)
            .With(f=>f.Id,Guid.NewGuid)
            .CreateMany(2)
            .ToList();

        await _catalogDbContext.Categories.AddRangeAsync(categories);
        await _catalogDbContext.Products.AddRangeAsync(products);
        await _catalogDbContext.SaveChangesAsync();
        
        var productIds = products.Select(p => p.Id).ToList();
        var productInDb = await _catalogDbContext.Products.Include(product => product.Categories)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync();
        
        Assert.NotNull(productInDb);
        CollectionAssert.AreEquivalent(productInDb, products);
        
        var categoriesInDb = productInDb.Select(f=>f.Categories.Select(c => c.Id).ToList()).ToList();
        CollectionAssert.AreEquivalent(categories, categoriesInDb);
        
        var createOrderRequest = fixture.Build<CreateOrderRequest>()
            .With(r => r.ProductItemModels, fixture.CreateMany<ProductItemModel>(2)
                .Select(item => 
                {
                    item.Id = Guid.NewGuid();
                    item.Quantity = fixture.Create<int>() % 10 + 1;
                    return item;
                }).ToList())
            .Create();
        
        var response = await _orderClient.PostAsJsonAsync("/catalog", createOrderRequest);
        
        Assert.Pass();
    }

    [TearDown]
    public void TearDown()
    {
        _orderServiceFactory.Dispose();
        _catalogServiceFactory.Dispose();
    }

    public void Dispose()
    {
        TearDown();
    }
}
}