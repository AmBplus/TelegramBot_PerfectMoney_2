using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TelegramBot_PerfectMoney.Model;

namespace TelegramBot_PerfectMoney.DataBase
{
    public class BotSettingMapping:IEntityTypeConfiguration<BotSetting>
    {
        public void Configure(EntityTypeBuilder<BotSetting> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.StopSelling);
            builder.Property(x => x.RuleText).IsRequired(false);
            builder.HasData(new List<BotSetting>()
            {
            new BotSetting() { id = 1,StopSelling = false ,RuleText = "متنی وجود ندارد"}
            });
        }
    }
}
