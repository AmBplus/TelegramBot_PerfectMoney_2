using PefectMoney.Shared.Helper;

namespace PefectMoney.Presentation.PresentationHelper.OperationBot
{
    public class ActionHelper
    {
            //  long Is Id
          public static Dictionary<long, BotAction> BotActions { get; } = new Dictionary<long, BotAction>();   
    }
    public record BotAction
    {
        public BotAction()
        {
            CreateDate = TimeHelper.DateTimeNow;
        }
        public DateTime CreateDate { get;  }
        public string ActionName { get; set; }
        public ActionStatus ActionStatus { get; set; }

        public string Message { get; set; } 
     

    }
    public enum ActionStatus
    {
        Finish,
        OnProccess,

        Cancel
        
    }
}
