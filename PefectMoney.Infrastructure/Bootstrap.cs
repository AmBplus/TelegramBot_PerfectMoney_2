
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.Settings;
using PefectMoney.Core.UseCase.VerifyCard;
using PefectMoney.Data.DataBase;


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

            services.Configure<ZibalPaymentSettings>(
            configuration.GetSection(ZibalPaymentSettings.Configuration));
            services.Configure<BotSettings>(
           configuration.GetSection(BotSettings.Configuration));
            services.AddDbContext<ITelContext, TelContext>(
             dbContextOptions => dbContextOptions
           .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
           // The following three options help with debugging, but should
           // be changed or removed for production.
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors());
         

            services.AddSingleton<IVerifyCardToken, VerifyCardToken>();
            services.AddTransient<IVerifyUserCard, VerifyUserCardHandler>();

            services.AddMediatR(cfg=>cfg.RegisterServicesFromAssemblies(typeof(UserModel).Assembly));



        }

    }
}
