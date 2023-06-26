using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_PerfectMoney.Model
{
    public class BankCart : Base
    {
        public string CartNumber { get; set; }
        public bool IsActive { get; set; }
        public long UserId { get; set; } // Foreign key property

        public UserModel User { get; set; } // Navigation property
    }
}
