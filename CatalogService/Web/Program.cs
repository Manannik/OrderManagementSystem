using Application.Extensions;
using OrderManagementSystem.Infrastructure.Extensions;
using Serilog;
using WebApplication1.Extensions;
using WebApplication1.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});
builder.Services.AddTransient<CatalogServiceExceptionHandlerMiddleware>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWeb();
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);

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
public partial class Program {}