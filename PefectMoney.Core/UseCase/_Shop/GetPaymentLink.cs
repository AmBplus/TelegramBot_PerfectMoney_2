using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Extensions;
using PefectMoney.Core.Model;
using PefectMoney.Core.UseCase.Notify;
using PefectMoney.Core.UseCase.UserAction;
using PefectMoney.Core.UseCase.ZibalPayment;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase._Shop
{
    public record GetVoicherPaymentLinkRequest(int Count, long BotChatId) : IRequest<ResultOperation<Uri>>
    {
       
    }
    public class GetVoicherPaymentLinkHandler : IRequestHandler<GetVoicherPaymentLinkRequest, ResultOperation<Uri>>
    {
        public GetVoicherPaymentLinkHandler(ILogger<GetVoicherPaymentLinkHandler> logger,IMediator mediator,ITelContext context)
        {
            Logger = logger;
            Mediator = mediator;
            Context = context;
        }

        public ILogger<GetVoicherPaymentLinkHandler> Logger { get; }
        public IMediator Mediator { get; }
        public ITelContext Context { get; }

        public async Task<ResultOperation<Uri>> Handle(GetVoicherPaymentLinkRequest request, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new GetOnTimeVoicherValueRequest());
            if(!result.IsSuccess)
            {
                return ResultOperation<Uri>.ToFailedResult(result.Message?.GetString());
            }
            var resultUserCards = await Mediator.Send(new GetUserCardsRequest(request.BotChatId));
            if(!resultUserCards.IsSuccess) {
                return ResultOperation<Uri>.ToFailedResult(result.Message?.GetString());
            }
            var userDto = await Mediator.Send(new GetUserByBotUserIdQueryRequest(request.BotChatId));
            if (userDto == null)
            {
                return ResultOperation<Uri>.ToFailedResult("کاربر پیدا نشد");
            }
            
            var dollarTotalAmount = Convert.ToDouble(request.Count) * result.Data.Dollars;
            var rialsTotalAmount = request.Count * result.Data.Rials;
            OrderEntity order = new OrderEntity() { 
                BotChatId = request.BotChatId,
                Count = request.Count,
                OrderStatus = (int)OrderStatus.Initial,
                ProductId =(int)ProductName.VoicherCode,
                TotalDollarPrice = dollarTotalAmount,
                TotalRialsPrice = rialsTotalAmount,
                Dollar = result.Data.Dollars,
                Rial = result.Data.Rials
            };
            try
            {
                Context.Add(order);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.InnerException?.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation<Uri>.ToFailedResult("خطایی پیش آمده بعدا امتحان کنید");

            }

            var resultGetPaymentLink  = await Mediator.Send(new ZibalPaymentRequest() {
                allowedCards = resultUserCards.Data.UserCards.Select(x => x.CardNumber).ToArray() ,
                amount = rialsTotalAmount,
                mobile = userDto.PhoneNumber,
                orderId = order.Id.ToString()
             }
            );
            if(resultGetPaymentLink == null || resultGetPaymentLink.IsSuccess == false) {
                return ResultOperation<Uri>.ToFailedResult(resultGetPaymentLink?.Message?.ToStringEnumerable() ?? "مشکلی پیش آمده بعدا تلاش کنید");
            }
            order.trackId = resultGetPaymentLink.Data.trackId;
            try
            {
                Context.Update(order);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.InnerException?.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation<Uri>.ToFailedResult( "مشکلی پیش آمده بعدا تلاش کنید");
            }
            return resultGetPaymentLink.Data.Uri.ToSuccessResult();
        }

        
    }
}
