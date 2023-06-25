using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Telegram.Bot.Types;
using TelegramBot_PerfectMoney.Model;

namespace TelegramBot_PerfectMoney.DataBase
{
    public class TelContext:DbContext
    {
    

        public DbSet<userModel> Users { get; set; }
        public DbSet<BotSetting> botSettings { get; set; }
        public TelContext(DbContextOptions<TelContext> dbContext) : base(dbContext)
        {

        }
   

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            var assembly = typeof(UserMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            
        }
    }
}
