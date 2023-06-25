using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Exceptions;
using TelegramBot_PerfectMoney;
using TelegramBot_PerfectMoney.OperationBot;
using TelegramBot_PerfectMoney.TelegramPresentation;

#region Add Dependency Injection

var builder = Host.CreateDefaultBuilder(args);
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


// catch (Exception e)
// {
//     Console.ForegroundColor = ConsoleColor.Red;
//     Console.Write("Error");
//     var ErrorMessage = e.ToString();
//     Console.ResetColor();
//     Console.WriteLine(ErrorMessage);
//     Console.WriteLine("\n\n Press any key for close program......");
//     Console.ReadKey();
// }
