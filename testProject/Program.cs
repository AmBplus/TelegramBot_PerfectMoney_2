
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using testProject;
using testProject.Settings;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.Sources.Clear();
              config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
              config.AddEnvironmentVariables();
});



builder.AddTelegramConfig();
var app = builder.Build();
 
var verify = app.Services.GetService<VerifyAccountSettings>();
var verify1 = app.Services.GetService<IOptions<VerifyAccountSettings>>();


Console.ReadLine();