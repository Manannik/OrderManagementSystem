using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Order.Application.Abstractions;
using Order.Application.Extensions;
using Order.Application.Models;
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
builder.Services.AddInfrastructure(builder.Configuration);
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

public partial class Program
{
    public class TestingConfiguration
    {
        public bool SkipMigration { get; set; }
    }

    static void MigrateDb(IApplicationBuilder app)
    {
        var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

        using var scope = scopeFactory.CreateScope();
        var services = scope.ServiceProvider;

        var testConfig = services.GetService<IOptions<TestingConfiguration>>();
        if (testConfig?.Value?.SkipMigration == true)
        {
            return;
        }

        var dbContext = services.GetRequiredService<OrderDbContext>();

        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
        }
    }
}