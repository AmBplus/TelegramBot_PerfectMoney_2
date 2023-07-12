

using PefectMoney.Shared.Helper;

namespace PefectMoney.Core.Model
{
    public class UserEntity: Base<long>
    {
        public UserEntity(long botChatId,string phoneNumber,int roleId) :base()
        {
            BotChatId = botChatId;
            PhoneNumber = phoneNumber;
            RoleId = roleId;
            CreationDate = TimeHelper.DateTimeNow;
            Active = true;
        }
        public UserEntity(string phoneNumber, int roleId) : base()
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
        public bool Active { get; set; } = true;
        public int RoleId { get; set; }
        public ICollection<BankCardEntity> BankAccountNumbers { get; set; }
        public RoleModel? Roles { get; set; }
     
    }
}
