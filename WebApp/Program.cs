using PefectMoney.Core.Settings;
using PefectMoney.Infrastructure;
using Telegram.Bot;
using WebApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ITelegramBotClient,TelegramBotClient>(x=> new TelegramBotClient(token: ""));

builder.Services.BootstrapInjectServices(builder.Configuration);

if (builder.Environment.EnvironmentName == Environments.Development)
{
    builder.Services.ConfigureWritable<BotSettings>(builder.Configuration.GetSection(BotSettings.Configuration), file: $"appsettings.{Environments.Development}.json");
}
else
{
    builder.Services.ConfigureWritable<BotSettings>(builder.Configuration.GetSection(BotSettings.Configuration));
}



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}