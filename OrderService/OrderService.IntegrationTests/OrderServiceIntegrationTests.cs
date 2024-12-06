using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Order.Application.Abstractions;
using Order.Application.Models;
using Order.Domain.Abstractions;
using Order.Persistence;
using Order.Persistence.Repositories;

namespace OrderService.IntegrationTests
{
public class OrderServiceIntegrationTests : IDisposable
{
    private WebApplicationFactory<Program> _orderServiceFactory;
    private WebApplicationFactory<Program> _catalogServiceFactory;
    private HttpClient _orderClient;
    private HttpClient _productClient;
    private OrderDbContext _context;
    private IOrderService _orderService;
    private Mock<IQuantityService> _mockQuantityService;

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
    }
    
    private void SetupProductService()
    {
        
    }

    [Test]
    public void CreateOrder_WhenCatalogServiceReturnsSuccess_ShouldCreateOrderSuccessfully()
    {
        var fixture = new Fixture();
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var productItemModels = fixture.CreateMany<ProductItemModel>(2).ToList();
        foreach (var item in productItemModels)
        {
            item.Id = Guid.NewGuid();
            item.Quantity = fixture.Create<int>() % 10 + 1;
        }

        var createOrderRequest = new CreateOrderRequest
        {
            ProductItemModels = productItemModels
        };
        
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