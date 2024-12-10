using Hangfire;
using HangfireBasicAuthenticationFilter;
using Infrastructure.Persistence.Extensions;
using OrderProcessingService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
// builder.Services.AddWeb<string>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard();
app.MapHangfireDashboard("/hangfire", new DashboardOptions()
{
    DashboardTitle = "OrderProcessingService",
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