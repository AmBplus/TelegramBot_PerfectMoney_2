using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PefectMoney.Core.Model;

namespace PefectMoney.Core.Model
{
    public class RoleName 
    {
        public const int Admin = 1;
        public const int Customer = 2;
    }
    public class RoleModel: Base<int>
    {
        
        public string? Role { get; set; }
        public ICollection<UserModel> Users { get; set; }

        public RoleModel()
        {
        }
    }
}
