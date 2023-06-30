
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.Settings;
using PefectMoney.Data.Mapping;

namespace PefectMoney.Data.DataBase
{
    public class TelContext:DbContext , ITelContext
    {

        private static string connectionString = "Server=localhost; User ID=root; Password=0903@m!rK; Database=TelBot";
        public DbSet<UserModel> Users { get; set; }
    
        public DbSet<BankCard> BankCards { get; set; }
        public DbSet<RoleModel> RoleModels{ get; set; }
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
