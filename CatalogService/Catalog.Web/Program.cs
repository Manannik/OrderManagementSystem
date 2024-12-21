using Application.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderManagementSystem.Infrastructure;
using OrderManagementSystem.Infrastructure.Extensions;
using Serilog;
using WebApplication.Extensions;
using WebApplication.Middlewares;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

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
builder.Services.Configure<TestingConfiguration>(options => options.SkipMigration = false);

var app = builder.Build();
MigrateDb(app);

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

        var dbContext = services.GetRequiredService<CatalogDbContext>();

        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
        }
    }
}