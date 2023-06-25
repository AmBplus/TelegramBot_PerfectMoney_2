using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_PerfectMoney.Model
{
    public class BotSetting
    {
        public long id { get; set; }
        public bool StopSelling { get; set; } = false;
        // public bool Repair { get; set; }
        public string  RuleText { get; set; }
        public BotSetting()
        {
            StopSelling = false;
            // Repair = false;
        }
    }
}
