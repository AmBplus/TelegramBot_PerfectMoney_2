using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Settings
{
    public class BotSettings
    {
        public long id { get; set; }
        public bool StopSelling { get; set; } = false;
        // public bool Repair { get; set; }
        public string RuleText { get; set; }

        public static string BotLaws { get; set; } = _botLaws ??= GetBotLaws();

        private static string _botLaws;
        private static string GetBotLaws()
        {

            StringBuilder laws = new StringBuilder();
            laws.AppendLine("استفاده از خدمات سایت منوط بر اشتراک گذاری شماره همراه است");
            laws.AppendLine("برای خرید لازم است ابتدا حداقل یک شماره کارت تایید شده داشته باشید");
            laws.AppendLine("شماره کارت هایی تایید میشوند که مالک آن ها شماره همراهی باشد که به اشتراک گذاشته شده");
            return laws.ToString();

        }

        public BotSettings()
        {
            StopSelling = false;
            // Repair = false;
        }
    }
}
