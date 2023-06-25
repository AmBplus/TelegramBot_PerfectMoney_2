using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace TelegramBot_PerfectMoney.Model
{
    public class userModel:Base
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserNameTelegram { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CodeId { get; set; }
        public string? ChatId { get; set; }
        public bool Active { get; set; }
        public long RoleId { get; set; }
        public RoleModel? Roles { get; set; }
        public userModel()
        {
            CreationDate = DateTime.Now;
            Active = true;
        }
    }
}
