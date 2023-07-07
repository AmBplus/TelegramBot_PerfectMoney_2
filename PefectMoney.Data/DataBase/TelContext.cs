
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Data.Mapping;

namespace PefectMoney.Data.DataBase
{
    public class TelContext:DbContext , ITelContext
    {
        public TelContext(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("MySqlConnection");
        }

        private  string connectionString { get; set; }
        public DbSet<UserEntity> Users { get; set; }
    
        public DbSet<BankCardEntity> BankCards { get; set; }
        public DbSet<RoleModel> RoleModels{ get; set; }
        public DbSet<VoicherCodeEntity> VoicherCodes { get ; set ; }
        public DbSet<OrderEntity> Orders { get ; set ; }

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
