using Hangfire;
using HangfireBasicAuthenticationFilter;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Extensions;
using Messaging.Kafka;
using Messaging.Kafka.Models;
using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Application.Extensions;
using OrderProcessingService.Infrastructure.Extensions;
using OrderProcessingService.Web.Extensions;
using OrderProcessingService.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddTransient<OrderProcessingServiceExceptionHandlerMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWeb<string>(builder.Configuration);

var app = builder.Build();
MigrateDb(app);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<OrderProcessingServiceExceptionHandlerMiddleware>();

app.UseAuthorization();

app.UseHangfireDashboard();
app.MapHangfireDashboard("/hangfire", new DashboardOptions()
{
    DashboardTitle = "Order-processing-service",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter()
        {
            User ="user",
            Pass = "user"
        }
    }
});

app.MapControllers();

app.Run();

static void MigrateDb(IApplicationBuilder app)
{
    var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

    using var scope = scopeFactory.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderProcessingDbContext>();
    dbContext.Database.Migrate();
}