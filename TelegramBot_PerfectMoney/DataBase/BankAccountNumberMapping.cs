using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBot_PerfectMoney.Model;

namespace TelegramBot_PerfectMoney.DataBase
{
    class BankAccountNumberMapping : IEntityTypeConfiguration<BankAccountNumber>
    {
        public void Configure(EntityTypeBuilder<BankAccountNumber> builder)
        {
            builder.HasOne(b => b.User)
                .WithMany(u => u.BankAccountNumbers)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
