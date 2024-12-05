using System.Net;
using System.Net.Http.Json;
using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Models;
using Application.Models;
using AutoFixture;
using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace IntegrationsTests
{
    public class ProductControllerEndToEndTests : IDisposable
    {
        private WebApplicationFactory<Program> _webHost;
        private HttpClient _client;
        private CatalogDbContext _context;

        [SetUp]
        public void SetupForController()
        {
            _webHost = new WebApplicationFactory<Program>()
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

                        _context = new CatalogDbContext(options);
                        services.AddSingleton(_ => _context);
                        services.AddScoped<IProductRepository, ProductRepository>();
                        services.AddScoped<ICategoryRepository, CategoryRepository>();

                        services.AddMediatR(configuration =>
                        {
                            configuration.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
                        });
                    });
                });

            _client = _webHost.CreateClient();

            _context.Products.RemoveRange(_context.Products);
            _context.Categories.RemoveRange(_context.Categories);
            _context.SaveChanges();
        }

        [Test]
        public async Task CreateProduct_ReturnsCreatedResponse()
        {
            var fixture = new Fixture();
            fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var categoryModelDtos = fixture.CreateMany<CategoryModelDto>(2).ToList();
            var request = fixture.Build<CreateProductRequest>()
                .With(req => req.Price, fixture.Create<decimal>() % 100 + 1)
                .With(req => req.Name, fixture.Create<string>().Substring(0, 10))
                .With(req => req.Description, fixture.Create<string>().Substring(0, 10))
                .With(req => req.Quantity, fixture.Create<int>() % 100)
                .With(req => req.CategoryModelDtos, categoryModelDtos)
                .Create();

            var categories = categoryModelDtos
                .Select(dto => fixture.Build<Category>()
                    .With(c => c.Id, dto.Id)
                    .Create())
                .ToList();

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            Assert.That(await _context.Categories.CountAsync(), Is.EqualTo(categories.Count));

            var response = await _client.PostAsJsonAsync("/catalog", request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var productInDb = await _context.Products.Include(product => product.Categories)
                .FirstOrDefaultAsync(f => f.Name == request.Name);

            Assert.NotNull(productInDb);
            Assert.That(productInDb.Name, Is.EqualTo(request.Name));
            Assert.That(productInDb.Description, Is.EqualTo(request.Description));
            Assert.That(productInDb.Price, Is.EqualTo(request.Price));
            Assert.That(productInDb.Quantity, Is.EqualTo(request.Quantity));

            var categoriesInDb = productInDb.Categories.Select(c => c.Name).ToList();
            CollectionAssert.AreEquivalent(request.CategoryModelDtos.Select(c => c.Id), categoriesInDb);
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