

using PefectMoney.Shared.Helper;

namespace PefectMoney.Core.Model
{
    public class UserModel: Base<long>
    {
        public UserModel(long botChatId,string phoneNumber,int roleId)
        {
            BotChatId = botChatId;
            PhoneNumber = phoneNumber;
            RoleId = roleId;
            CreationDate = TimeHelper.DateTimeNow;
            Active = true;
        }
        public UserModel(string phoneNumber, int roleId)
        {
           
            PhoneNumber = phoneNumber;
            RoleId = roleId;
            CreationDate = TimeHelper.DateTimeNow;
            Active = true;
        }


        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserNameTelegram { get; set; }
        public string? PhoneNumber { get; set; }
        public long BotChatId { get; set; }  
        public bool Active { get; set; }
        public int RoleId { get; set; }
        public ICollection<BankCard> BankAccountNumbers { get; set; }
        public RoleModel? Roles { get; set; }
     
    }
}
