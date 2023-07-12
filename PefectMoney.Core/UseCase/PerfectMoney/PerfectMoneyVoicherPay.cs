using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PefectMoney.Core.Data;
using PefectMoney.Core.Extensions;
using PefectMoney.Core.Model;
using PefectMoney.Core.Settings;
using PefectMoney.Core.UseCase.Notify;
using PefectMoney.Core.UseCase.ZibalPayment;


namespace PefectMoney.Core.UseCase.PerfectMoney
{
    public record PerfectMoneyVoicherPayRequest : IRequest
    {
        public OrderDto Order { get; set; }

    }
    public class PerfectMoneyVoicherPayHandler:IRequestHandler<PerfectMoneyVoicherPayRequest>
    {
        public PerfectMoneyVoicherPayHandler(ILogger<PerfectMoneyVoicherPayHandler> logger,
            ITelContext context,IOptions<PerfectMonneySettings> options,IMediator mediator)
        {
            Logger = logger;
            Context = context;
            Mediator = mediator;
            PerfectMonneySettings = options.Value;
        }

        public ILogger<PerfectMoneyVoicherPayHandler> Logger { get; }
        public ITelContext Context { get; }
        public IMediator Mediator { get; }
        public PerfectMonneySettings PerfectMonneySettings { get; }

        public async Task Handle(PerfectMoneyVoicherPayRequest request, CancellationToken cancellationToken)
        {
            var order = await Context.Orders.FirstOrDefaultAsync(x => x.Id == request.Order.Id);
            if (order == null) return;
            if(order.OrderStatus == (int)OrderStatus.Finish)
            {
                PerfectmoneyModel perfectmoneyModel =
                    new PerfectmoneyModel(PerfectMonneySettings.AccountID
                    , PerfectMonneySettings.PassPhrase, PerfectMonneySettings.PayerAccount);
               var result = perfectmoneyModel.CreateVoucher(order.TotalDollarPrice);
                // Rise Event 

                // validateResult
                try
                {
                    await Mediator.Publish(new NotifyAdminRequest($"پرفکت مانی :{StringExtensionHelper.CreateString(result?.Message)}"));
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message, e.InnerException?.Message, e.StackTrace , e.InnerException?.StackTrace);
                }


                try
                {
                    if(result.IsSuccess)
                    {
                        Context.VoicherCodes.Add(new VoicherCodeEntity {
                            VoicherCode = StringExtensionHelper.CreateString(result.Message)
                            ,OrderId = order.Id ,UserBotChatId = order.BotChatId});
                        await Context.SaveChangesAsync(); 

                    }
                    else
                    {
                      await Mediator.Publish(new NotifyPaymentToUserRequest() { BotChatId= order.BotChatId ,OrderId = order.Id ,Message = "سفارش شما با مشکل مواجه شده لطفا با ادمین تماس حاصل فرمایید"});
                    }
                  
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message, e.InnerException?.Message, e.StackTrace , e.InnerException?.StackTrace);
                    await Mediator.Publish(new NotifyAdminRequest($"پرفکت مانی :{StringExtensionHelper.CreateString(result?.Message)}"));
                    
                }
            }
            
        }
    }
}
