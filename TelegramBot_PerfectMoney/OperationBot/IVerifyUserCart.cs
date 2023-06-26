using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_PerfectMoney.OperationBot
{
    public interface IVerifyUserCart
    {
        public bool Verify(string phoneNumber, string CartNumber);
    }
}
