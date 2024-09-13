using Application.BusinessLogic.Commands.CreateProduct;
using Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Repository;
using Serilog;
using WebApplication1.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers();
builder.Services.AddSingleton(typeof(CatalogServiceExceptionHandlerMiddleware));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssemblyContaining<CreateProductCommand>());
builder.Services.AddScoped<IProductRepository, ProductRepositoryy>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogServiceConnectionString"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseMiddleware<CatalogServiceExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
