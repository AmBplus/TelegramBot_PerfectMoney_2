

namespace PefectMoney.Core.Model
{
    public class UserModel:Base
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserNameTelegram { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CodeId { get; set; }
        public string? ChatId { get; set; }
        public bool Active { get; set; }
        public long RoleId { get; set; }
        public ICollection<BankCard> BankAccountNumbers { get; set; }
        public RoleModel? Roles { get; set; }
        public UserModel()
        {
            CreationDate = DateTime.Now;
            Active = true;
        }
    }
}
