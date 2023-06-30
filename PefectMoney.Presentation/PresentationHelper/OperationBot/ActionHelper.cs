namespace PefectMoney.Presentation.PresentationHelper.OperationBot
{
    public class ActionHelper
    {
            //  long Is Id
          public static Dictionary<long, BotAction> BotActions { get; set; }
    }
    public record BotAction
    {
        public string ActionName { get; set; }
        public ActionStatus ActionStatus { get; set; }
    }
    public enum ActionStatus
    {
        Finish,
        OnProccess,
        Cancel
        
    }
}
