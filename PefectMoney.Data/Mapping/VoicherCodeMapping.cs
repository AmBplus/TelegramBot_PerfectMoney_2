using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PefectMoney.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Data.Mapping
{
    public class VoicherCodeMapping : IEntityTypeConfiguration<VoicherCodeEntity>
    {
        public void Configure(EntityTypeBuilder<VoicherCodeEntity> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(x => x.Order).WithOne(x => x.VoicherCode);
        }
    }
}
