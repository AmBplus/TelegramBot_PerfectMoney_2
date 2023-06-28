using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PefectMoney.Core.Model;

namespace PefectMoney.Core.Model
{
    public class RoleModel:Base
    {
        public string? Role { get; set; }
        public ICollection<UserModel> Users { get; set; }

        public RoleModel()
        {
        }
    }
}
