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
    public sealed class MapProduct : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x=>x.Orders).WithOne(x => x.Product).HasForeignKey(x=>x.ProductId);
            builder.HasData( new ProductEntity() { Id = (int)ProductName.VoicherCode ,Name = ProductName.VoicherCode.ToString()});
        }
    }
}
