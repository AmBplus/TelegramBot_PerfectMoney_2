using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Settings;
using PefectMoney.Core.UseCase.VerifyCard;
using PefectMoney.Data.DataBase;
using static System.Formats.Asn1.AsnWriter;

namespace PefectMoney.Infrastructure
{
    public static class Bootstrap
    {
        private static string connectionString = "Server=localhost; User ID=root; Password=0903@m!rK; Database=TelBot";

        public static void BootstrapInjectServices(this IServiceCollection services, IConfiguration configuration)
        {


            var con = configuration.GetConnectionString("MySqlConnection");
       
            
            services.Configure<VerifyAccountSettings>(
             configuration.GetSection(VerifyAccountSettings.Configuration));

            

            services.AddDbContext<ITelContext, TelContext>(
             dbContextOptions => dbContextOptions
           .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
           // The following three options help with debugging, but should
           // be changed or removed for production.
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors());
            //services.AddSingleton<TelegramBot>();
            //services.AddScoped<IOperationTelegramBot, OperationTelegramBot>();
            /*services.Configure<VerifyAccountSettings>(hostContext.Configuration.GetSection("VerifyNumberAndCart"));*/

            services.AddSingleton<IVerifyCardToken, VerifyCardToken>();
            services.AddTransient<IVerifyUserCard, VerifyUserCardHandler>();



        }
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable RCS1110 // Declare type inside namespace.
        public class BotConfiguration1
#pragma warning restore RCS1110 // Declare type inside namespace.
#pragma warning restore CA1050 // Declare types in namespaces
        {
            private string _address;
            private string _clientId;
            private string _clientSecret;
          
            private string _scopes;
            private string _nid;
            public static readonly string Configuration = "BotConfiguration1";

            public string BotToken { get; init; } = default!;
            public string HostAddress { get; init; } = default!;
            public string Route { get; init; } = default!;
            public string SecretToken { get; init; } = default!;
            public string Address
            {
                get;
                set;
            } = default!;


            public string ClientId
            {
                get { return _clientId; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("ClientId cannot be null, empty, or whitespace.");
                    }
                    _clientId = value;
                }
            }


            public string ClientSecret
            {
                get { return _clientSecret; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("ClientSecret cannot be null, empty, or whitespace.");
                    }
                    _clientSecret = value;
                }
            }


        


            public string Scopes
            {
                get { return _scopes; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("Scopes cannot be null, empty, or whitespace.");
                    }
                    _scopes = value;
                }
            }


            public string Nid
            {
                get { return _nid; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("Nid cannot be null, empty, or whitespace.");
                    }
                    _nid = value;
                }
            }
        }


    }
}
