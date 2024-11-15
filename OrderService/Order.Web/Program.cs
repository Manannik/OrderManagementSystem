using Microsoft.EntityFrameworkCore;
using Order.Application.Abstractions;
using Order.Application.Extensions;
using Order.Infrastructure.Extensions;
using Order.Infrastructure.Services;
using Order.Persistence;
using Order.Persistence.Extensions;
using Order.Web.Extensions;
using Order.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddTransient<OrderServiceExceptionHandlerMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddWeb();
builder.Services.AddInfrastructure<Order.Domain.Entities.Order>(builder.Configuration.GetSection("Kafka:Order"));

builder.Services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(o =>
{
    o.BaseAddress = new Uri("https://localhost:7186/Catalog/");
});

var app = builder.Build();
MigrateDb(app);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<OrderServiceExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void MigrateDb(IApplicationBuilder app)
{
    var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

    using var scope = scopeFactory.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    dbContext.Database.Migrate();
}