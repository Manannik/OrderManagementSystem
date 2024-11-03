using Order.Domain.Abstractions;
using Order.Persistence.Extensions;
using Order.Persistence.Services;
using Order.Web.Extensions;
using Order.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddWeb();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddTransient<OrderServiceExceptionHandlerMiddleware>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddHttpClient<ICatalogServiceClient, CatalogServiceClient>(o =>
{
    o.BaseAddress = new Uri("https://localhost:44391/Catalog/");
});

var app = builder.Build();

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