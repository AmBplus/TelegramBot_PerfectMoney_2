namespace PefectMoney.Presentation.PresentationHelper.OperationBot;
    public class PaginationHelper
    {
        public static Dictionary<long, UserPaginationBot>  UserPaginationHelepr  {get ;} = new Dictionary<long,UserPaginationBot>();
    }

public record UserPaginationBot
{
    public int PageSize { get; set; } = 10;
    public int Page { get; set; } = 0;
    public bool IsLast { get; set; } = false;
}

