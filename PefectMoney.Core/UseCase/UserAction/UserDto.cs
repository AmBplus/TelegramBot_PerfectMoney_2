using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.UserAction
{
    public record UserDto
    {
        public long Id{ get; set; }
        public long BotChatId { get; set; }
        public RoleDto Roles { get; set; }
        public string PhoneNumber { get; set; }
    }
}
