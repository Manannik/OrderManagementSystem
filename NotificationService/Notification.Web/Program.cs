using Notification.Web.Extensions;
using Telegram.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ContainerConfigurator.Configure(builder.Configuration, builder.Services);
builder.Services.AddHostedService<WebHookConfigurator>();
builder.Services.ConfigureTelegramBotMvc();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWeb<string>(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();