using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_PerfectMoney.Model
{
    public class RoleModel:Base
    {
        public string? Role { get; set; }
        public ICollection<userModel> Users { get; set; }

        public RoleModel()
        {
        }
    }
}
