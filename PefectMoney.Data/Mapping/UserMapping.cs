using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PefectMoney.Core.Model;

namespace PefectMoney.Data.Mapping
{
    public class UserMapping : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FirstName).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.LastName).HasMaxLength(200).IsRequired(false);
      
            builder.Property(x => x.Active);
            builder.Property(x => x.CreationDate);
            builder.Property(x => x.PhoneNumber).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.UserNameTelegram).IsRequired(false).HasMaxLength(200);
            builder.HasMany(u => u.BankAccountNumbers)
            .WithOne(u => u.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Roles).WithMany(x => x.Users).HasForeignKey(x => x.RoleId);
            builder.HasData(new List<UserEntity>()
            {
                new UserEntity("+989394059810",RoleName.Admin) { Id =1 },
                new UserEntity("+989308505480",RoleName.Admin) { Id =2 }
            });
        }
    }
}
