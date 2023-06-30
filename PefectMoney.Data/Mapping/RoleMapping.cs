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
    public class RoleMapping : IEntityTypeConfiguration<RoleModel>
    {
        public void Configure(EntityTypeBuilder<RoleModel> builder)
        {
            builder.HasKey(x => x.id);
            builder.HasMany(x => x.Users).WithOne(x => x.Roles).HasForeignKey(x => x.RoleId);
            builder.HasData(new List<RoleModel>()
            {
                new RoleModel() { CreationDate = DateTime.Now, id = 1, Role = RoleName.Admin.ToString()},
                new RoleModel() { CreationDate = DateTime.Now, id = 2, Role = RoleName.Customer.ToString() },
            });
        }
    }
}
