using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using testProject.Settings;


namespace testProject
{
    public static class Bootstrap
    {

        public static void AddTelegramConfig(this IHostBuilder host)
        {
            host.ConfigureServices((hostContext, services) =>
            {
                //var verifyAccount = hostContext.Configuration.GetSection("VerifyNumberAndCart").Get<VerifyAccountSettings>();
                //services.AddSingleton<VerifyAccountSettings>(verifyAccount);
               var t =  hostContext.Configuration;
                var ver = hostContext.Configuration.GetSection("VerifyAccountSettings");
                var ver1 = hostContext.Configuration.GetSection("VerifyAccountSettings").Get<VerifyAccountSettings>();
                 services.Configure<VerifyAccountSettings>(ver);
             
                

                // services.AddDbContext<TelContext>(x=>x.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));
            });
        }
    }
}
