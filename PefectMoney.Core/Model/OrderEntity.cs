using Microsoft.VisualBasic;
using PefectMoney.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Model
{
    public class OrderEntity
    {
        public OrderEntity()
        {
            CreationTime = TimeHelper.DateTimeNow;
        }
        public DateTime CreationTime { get; set; }
        public long Id { get; set; }
        public string trackId { get; set; }
        public double TotalDollarPrice { get; set; }
        public double TotalRialsPrice { get; set; }

        public int OrderStatus { get; set; }
        public int Count { get; set; }
        public ProductEntity Product {get;set;}
        public long ProductId { get; set; }
        public long BotChatId { get; set; }
     
        public int Rial { get; set; }
        public double Dollar { get; set; }

        public VoicherCodeEntity VoicherCode { get; set; }
        public long VoicherCodeId { get; set; }
     
    }

    public enum OrderStatus : int
    {
        Unknown = 0,
        OnProcess = 5 ,
        Finish = 10,
        TimeOut = 15 ,
        Cancel = 20 ,
        Initial = 25
    }
}
