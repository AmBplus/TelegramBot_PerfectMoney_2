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
        public VoicherCodeEntity()
        {
            CreationDate = TimeHelper.DateTimeNow;
        }
        public string VoicherCode { get; set; }
        public long OrderId { get; set; }
        public OrderEntity Order { get; set; }
        
        public long BotChatId { get; set; }
    }
}
