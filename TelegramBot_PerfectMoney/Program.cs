using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Exceptions;
using TelegramBot_PerfectMoney;
using TelegramBot_PerfectMoney.TelegramPresentation;
using Microsoft.Extensions.Configuration;

#region Add Dependency Injection

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration((hostingContext, config) =>
{

});




builder.AddTelegramConfig();
var app = builder.Build();

#endregion




var TokenPathFile = Path.Combine(AppContext.BaseDirectory, "TokenBot.txt");
var Token = File.ReadAllText(TokenPathFile);
Console.TreatControlCAsInput = true;
var telegram = app.Services.GetService<TelegramBot>();

    await telegram.Run(Token);
    Console.WriteLine("\n------------------------------------------------------------\n");
    Console.WriteLine("For stop the prgoram press ESC");

    while (true)
    {
        ConsoleKeyInfo keyinfo = Console.ReadKey(true);
        if (keyinfo.Key == ConsoleKey.Escape)
        {
            telegram.Stop();
            break;
        }
    }
  