using System.Net.Http.Json;
using AutoFixture;
using Confluent.Kafka;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Order.Application.Abstractions;
using Order.Application.Helpers;
using Order.Application.Models;
using Order.Application.Models.Kafka;
using Order.Domain.Abstractions;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Order.Infrastructure.Services;
using Order.Persistence;
using Order.Persistence.Repositories;
using Order.Web.Controllers.Validators;

namespace OrderService.IntegrationTests
{
    public class OrderServiceIntegrationTestsEndToEnd : IDisposable
    {
        private WebApplicationFactory<Program> _orderServiceFactory;
        private HttpClient _productClient;
        private HttpClient _orderClient;
        private OrderDbContext _orderDbContext;
        private IOrderService _orderService;
        private IServiceScope _orderServiceScope;
        private Mock<IQuantityService> _mockQuantityService = new Mock<IQuantityService>();
        private Mock<IProducer<string, string>> mockProducer = new Mock<IProducer<string, string>>();
        private Mock<ICatalogServiceClient> _mockCatalogServiceClient  = new Mock<ICatalogServiceClient>();

        [SetUp]
        public void Setup()
        {
            var testConfig = new Dictionary<string, string>
            {
                { "TestingConfiguration:SkipMigration", "true" },
                { "ServiceUrls:CatalogService", "http://localhost" }
            };

            var testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(testConfig)
                .Build();

            _orderServiceFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddInMemoryCollection(testConfig);
                    });

                    builder.ConfigureServices(services =>
                    {
                        var descriptorsToRemove = services.Where(d =>
                                d.ServiceType == typeof(OrderDbContext) ||
                                d.ServiceType == typeof(IOrderRepository) ||
                                d.ServiceType == typeof(IOrderService) ||
                                d.ServiceType == typeof(ICatalogServiceClient) ||
                                d.ServiceType == typeof(IQuantityService))
                            .ToList();

                        foreach (var descriptor in descriptorsToRemove)
                        {
                            services.Remove(descriptor);
                        }

                        services.AddDbContext<OrderDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryOrderDB");
                        });

                        services.AddSingleton(_mockQuantityService.Object);
                        services.AddScoped<IOrderService, Order.Application.Services.OrderService>();
                        services.AddScoped<IOrderRepository, OrderRepository>();
                        services.AddScoped<IQuantityService, QuantityService>();
                        services.AddSingleton<IProducer<string, string>>(mockProducer.Object);
                        services.AddSingleton(_mockCatalogServiceClient.Object);
                    });
                });

            _orderClient = _orderServiceFactory.CreateClient();

            _orderServiceScope = _orderServiceFactory.Services.CreateScope();

            _orderDbContext = _orderServiceScope.ServiceProvider.GetRequiredService<OrderDbContext>();

            _orderDbContext.Database.EnsureCreated();
            _orderDbContext.Orders.RemoveRange(_orderDbContext.Orders);
            _orderDbContext.SaveChanges();
        }
        [Test]
        public async Task CreateOrder_WhenCatalogServiceReturnsSuccess_ShouldCreateOrderSuccessfully()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var createOrderRequest = fixture.Build<CreateOrderRequest>()
                .With(r => r.ProductItemModels, fixture.CreateMany<ProductItemModel>(2)
                    .Select(item =>
                    {
                        item.Id = Guid.NewGuid();
                        item.Quantity = fixture.Create<int>() % 10 + 1;
                        return item;
                    }).ToList())
                .Create();

            var productItems = createOrderRequest.ProductItemModels.Select(f => new ProductItem
            {
                Id = f.Id,
                Quantity = f.Quantity,
                Price = fixture.Create<int>() % 10 + 1.0m
            }).ToList();

            var result = Result<List<ProductItem>, (Guid id, string Message, int StatusCode)>.Success(productItems);
            
            // не могу определить причину
            _mockQuantityService.Setup(service => service.TryChangeQuantityAsync(
                    createOrderRequest.ProductItemModels,
                    CancellationToken.None))
                .ReturnsAsync(result);
            
            _mockQuantityService.Verify(service => service.TryChangeQuantityAsync(
                createOrderRequest.ProductItemModels,
                CancellationToken.None), Times.Once);
            
            mockProducer.Setup(p => p.ProduceAsync(
                    It.IsAny<string>(),
                    It.IsAny<Message<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeliveryResult<string, string>
                {
                    Status = PersistenceStatus.Persisted,
                    Offset = 0
                });

            // Act
            var response = await _orderClient.PostAsJsonAsync("/order", createOrderRequest);

            // Assert
            mockProducer.Verify(p => p.ProduceAsync(
                    It.IsAny<string>(),
                    It.IsAny<Message<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            var orderInDb = await _orderDbContext.Orders.Include(o => o.ProductItems).SingleOrDefaultAsync();

            Assert.NotNull(orderInDb, "ЗАКАЗ НЕ БЫЛ СОЗДАН В БАЗЕ ДАННЫХ");
            Assert.AreEqual(productItems.Count, orderInDb.ProductItems.Count,
                "НЕ БЬЕТСЯ КОЛИЧЕСТВО PRODUCTITEMS В БД И В REQUEST");
            Assert.That(orderInDb.Cost, Is.EqualTo(productItems.Sum(p => p.Quantity * p.Price)), "ЦЕНА НЕКОРРЕКТНА");
            Assert.IsTrue(response.IsSuccessStatusCode, "HTTP СТАТУС КОД НЕКОРРЕКТЕН");
        }

        [TearDown]
        public void TearDown()
        {
            _orderServiceFactory.Dispose();
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}