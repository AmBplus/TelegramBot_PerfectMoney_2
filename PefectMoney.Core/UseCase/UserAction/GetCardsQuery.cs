using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Shared.Utility.ResultUtil;
using Microsoft.EntityFrameworkCore; 


namespace PefectMoney.Core.UseCase.UserAction
{
    public record GetUserCardsRequest(long ChatId) : IRequest<ResultOperation<GetUserCardsResponse>>
    {
     
    }
    public record GetUserCardsResponse(long ChatId)
    {
     
        public IList<UserCardsResponseDto> UserCards { get; set; } 
    }
    public class GetUserCardsHandler : IRequestHandler<GetUserCardsRequest,ResultOperation<GetUserCardsResponse>>
    {
        public GetUserCardsHandler(ITelContext context,ILogger<GetUserCardsHandler> logger)
        {
            Context = context;
            Logger = logger;
        }

        public ITelContext Context { get; }
        public ILogger<GetUserCardsHandler> Logger { get; }

        public async Task<ResultOperation<GetUserCardsResponse>> Handle(GetUserCardsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = Context.Users.Where(x => x.BotChatId == request.ChatId).Include(x => x.BankAccountNumbers).Select(x => new GetUserCardsResponse(x.BotChatId)
                {
                    UserCards = x.BankAccountNumbers.Where(b => b.IsActive)
                   .Select(b =>
                     new UserCardsResponseDto(b.CartNumber) { }).ToList(),

                }).FirstOrDefault();
              if(result==null)
                {
                     return ResultOperation<GetUserCardsResponse>.ToFailedResult("کاربر یافت نشد");
                }
                return result.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                return ResultOperation<GetUserCardsResponse>.ToFailedResult("مشکلی پیش آمده با ادمین تماس حاصل فرمایید");
            }
          
        }
    }
    public record UserCardsResponseDto(string CardNumber)
    {
       
    }
}
