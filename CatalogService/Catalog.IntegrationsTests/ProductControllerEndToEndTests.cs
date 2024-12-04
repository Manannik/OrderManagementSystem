using System.Net;
using System.Net.Http.Json;
using Application.BusinessLogic.Commands.CreateProduct;
using Application.BusinessLogic.Models;
using Application.Models;
using AutoFixture;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;

namespace IntegrationsTests;

public class ProductControllerEndToEndTests : IDisposable
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
                        //options.UseNpgsql("Host=127.0.0.1;Username=postgres;Password=test;Database=testDb;Encoding=UTF8;");
                    });
                    services.AddMediatR(configuration =>
                    {
                        configuration.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
                    });
                    services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
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
        //не получается решить проблему обращения к разным БД в handler и в тесте
        
        var fixture = new Fixture();
        fixture.Customize<DateTime>(c => c.FromFactory(() => DateTime.UtcNow));
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var categoryModelDto = fixture.CreateMany<CategoryModelDto>(2).ToList();
        fixture.Customize<CreateProductRequest>(c => c
                .With(req => req.Price, fixture.Create<decimal>() % 100 + 1)
                .With(req => req.Name, fixture.Create<string>().Substring(0, 10))
                .With(req => req.Description, fixture.Create<string>().Substring(0, 10))
                .With(req => req.Quantity, fixture.Create<int>() % 100)
                .With(req => req.CategoryModelDtos, fixture.CreateMany<CategoryModelDto>(2).ToList())
        );
        
        var request = fixture.Create<CreateProductRequest>();
        
        var categories = categoryModelDto
            .Select(dto => fixture.Build<Category>()
                .With(c => c.Id, dto.Id)
                .Create())
            .ToList();

        var context = _serviceScope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        
        Assert.That(await context.Categories.CountAsync(), Is.EqualTo(categories.Count));
        
        var response = await _client.PostAsJsonAsync("/catalog", request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var productInDb = await context.Products.Include(product => product.Categories)
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