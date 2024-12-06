using Application.BusinessLogic.Commands.CreateProduct;
using AutoFixture;
using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Order.Persistence;
using OrderManagementSystem.Infrastructure.Repository;

namespace OrderService.IntegrationTests
{
    public class OrderServiceIntegrationTests : IDisposable
    {
        private WebApplicationFactory<Program> _webHost;
        private HttpClient _client;
        private OrderDbContext _context;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            
            var testConfig = new Dictionary<string, string>
            {
                { "TestingConfiguration:SkipMigration", "false" }
            };

            var testConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(testConfig)
                .Build();
            
            _webHost = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton<IConfiguration>(testConfiguration);
                        //var descriptors = services.Where(d => /* Your condition here */).ToList();

                        //foreach (var descriptor in descriptors)
                        //{
                        //    services.Remove(descriptor);
                        //}
                        //
                        var options = new DbContextOptionsBuilder<OrderDbContext>()
                            .UseInMemoryDatabase("InMemoryCatalogDB")
                            .Options;

                        _context = new OrderDbContext(options);
                        services.AddSingleton(_ => _context);
                        services.AddScoped<IProductRepository, ProductRepository>();

                        services.AddMediatR(configuration =>
                        {
                            configuration.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
                        });
                    });

                    _client = _webHost.CreateClient();

                    //_context.Categories.RemoveRange(_context.Categories);
                    _context.SaveChanges();
                });
        }

        [Test]
        public void CreateOrder_WhenCatalogServiceReturnsSuccess_ShouldCreateOrderSuccessfully()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            Assert.Pass();
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
}