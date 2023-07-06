using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PefectMoney.Core.Model;

namespace PefectMoney.Data.Mapping
{
    class BankAccountNumberMapping : IEntityTypeConfiguration<BankCardEntity>
    {
        public void Configure(EntityTypeBuilder<BankCardEntity> builder)
        {
            builder.HasOne(b => b.User)
                .WithMany(u => u.BankAccountNumbers)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
