using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PefectMoney.Core.Model;

namespace PefectMoney.Core.Model
{
    public class BankCard : Base<long>
    {
        public string CartNumber { get; set; }
        public bool IsActive { get; set; }
        public long UserId { get; set; } // Foreign key property

        public UserModel User { get; set; } // Navigation property
    }
}
