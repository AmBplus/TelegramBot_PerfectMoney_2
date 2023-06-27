using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using TelegramBot_PerfectMoney.Model;
using TelegramBot_PerfectMoney.OperationBot;
using TelegramBot_PerfectMoney.TelegramPresentation;

namespace TelegramBot_PerfectMoney.DataBase
{
    public class TelContext:DbContext
    {

        private static string connectionString = "Server=localhost; User ID=root; Password=126543210mM$; Database=TelBot";
        public DbSet<UserModel> Users { get; set; }
        public DbSet<BotSetting> botSettings { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
         optionsBuilder
           .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
           // The following three options help with debugging, but should
           // be changed or removed for production.
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors();
      
            base.OnConfiguring(optionsBuilder); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            var assembly = typeof(UserMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            
        }
    }
}
