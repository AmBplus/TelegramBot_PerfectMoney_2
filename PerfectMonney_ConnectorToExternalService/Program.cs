using Microsoft.Extensions.Options;
using PerfectMonney_ConnectorToExternalService.Services;
using PerfectMonney_ConnectorToExternalService.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<BotSettings>(builder.Configuration.GetSection(BotSettings.Configuration));
    if (builder.Environment.EnvironmentName == Environments.Development)
{
    builder.Services.ConfigureWritable<BotSettings>(builder.Configuration.GetSection(BotSettings.Configuration), file: $"appsettings.{Environments.Development}.json");
}
else
{
    builder.Services.ConfigureWritable<BotSettings>(builder.Configuration.GetSection(BotSettings.Configuration));
}
;
builder.Services.AddScoped<IMapChanges, MapChanges>();
builder.Services.AddControllers();

var app = builder.Build();


var serviceProvider = builder.Services.BuildServiceProvider();
var optionMonitor = app.Services.GetRequiredService<IOptionsMonitor<BotSettings>>();
optionMonitor.OnChange(settings =>
{
    var OptionSettings = app.Services.GetRequiredService<IOptions<BotSettings>>(); // Even After Change Nothing Will Change
    var optionMonitorSettings = optionMonitor.CurrentValue; // Even After Change Nothing Will Change
    var SnapShotOptionSettings = serviceProvider.GetRequiredService<IOptionsSnapshot<BotSettings>>(); // The Value Changed

    OptionSettings.Value.AboutUs = SnapShotOptionSettings.Value.AboutUs; // Or You Map Like This Or  Create a Extension Or Other Ways ...


});





// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
