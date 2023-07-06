using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.Settings;
using PefectMoney.Shared.Helper;
using PefectMoney.Shared.Utility.ResultUtil;

namespace PefectMoney.Core.UseCase._Shop
{
    public record ValidateBasketRequest(string trackId) : IRequest<ResultOperation>
    {

    }
    public class ValidateBasketHandler : IRequestHandler<ValidateBasketRequest, ResultOperation>
    {
        public ValidateBasketHandler(ITelContext context,ILogger<ValidateBasketHandler> logger,IOptions<BotSettings> options)
        {
            Context = context;
            Logger = logger;
            BotSettings = options.Value ;
        }

        public ITelContext Context { get; }
        public ILogger<ValidateBasketHandler> Logger { get; }
        public BotSettings BotSettings { get; }

        public async Task<ResultOperation> Handle(ValidateBasketRequest request, CancellationToken cancellationToken)
        {
            if(request == null)
            {
                return ResultOperation.ToFailedResult("درخواست خالیست");
            }
            try
            {
                var result = await  Context.Orders.FirstOrDefaultAsync(x => x.trackId == request.trackId);
                if(result == null) {
                    return ResultOperation.ToFailedResult("چنین سبد خریدی پیدا نشد");
                }
                if(result.OrderStatus == (long)OrderStatus.OnProcess || result.OrderStatus == (long)OrderStatus.Initial)
                {
                    DateTime expireDate = result.CreationTime.AddMinutes(BotSettings.ValidBasketTime);
                    if(TimeHelper.DateTimeNow < expireDate)
                    {
                        result.OrderStatus = (int)OrderStatus.TimeOut;
                        Context.Update(result);
                        await Context.SaveChangesAsync();
                        return ResultOperation.ToFailedResult("مدت زمان سبد به پایان رسیده");

                    }
                    return ResultOperation.ToSuccessResult();
                }
                return ResultOperation.ToFailedResult($"وضعیت سبد {result.OrderStatus.ToString()}");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.InnerException?.Message);
                return ResultOperation.ToFailedResult();
            }
           
            
        }
    }
}
