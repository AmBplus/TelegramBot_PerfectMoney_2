using PefectMoney.Core.UseCase.UserAction;
using PefectMoney.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace PefectMoney.Core.Model
{
    public class VoicherCodeEntity : Base<long>
    {
        public VoicherCodeEntity() :base()
        {
        }
        public string VoicherCode { get; set; }
        
        public long UserBotChatId { get; set; }
        public OrderEntity Order { get; set; }
        public long OrderId { get; set; }


    }
}
