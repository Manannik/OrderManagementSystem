using System.Net;
using System.Text;
using Application.BusinessLogic.Models;
using Application.Models;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using OrderManagementSystem.Infrastructure;

namespace IntegrationsTests;

public class ProductControllerTests : IDisposable
{
    private WebApplicationFactory<Program> _webHost;
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
    }

    [Test]
    public async Task CreateProductController_ReturnsSuccess()
    {
        // Arrange
        var fixture = new Fixture();
        
        fixture.Customize<CreateProductRequest>(f =>
            f.With(request => request.CategoryModelDtos, fixture.CreateMany<CategoryModelDto>(2).ToList())
        );
        
        var product = fixture.Create<CreateProductRequest>();

        fixture.Customize<List<Category>>(f =>
            f.Do(categories =>
            {
                var categoryDtos = product.CategoryModelDtos.ToList();
                for (int i = 0; i < categories.Count && i < categoryDtos.Count; i++)
                {
                    categories[i].Id = categoryDtos[i].Id;
                }
            })
        );
        
        var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content
            });
        
        var client = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost")
        };

        // Act
        var response = await client.PostAsync("/catalog", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
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