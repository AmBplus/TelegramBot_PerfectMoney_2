using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_PerfectMoney.OperationBot
{
    
    public class VerifyUserCart
    {
        public string Address { get; set; } 
        public string ClinetId { get; set; }
        public string TrackId { get; set; }
        public string Token { get;set; }
        public bool Verify(string phoneNumber,string CartNumber)
        {
            throw new NotImplementedException();
        }
        
    }
    public class ResponseVerifyUserCartDto
    {

    }
}
