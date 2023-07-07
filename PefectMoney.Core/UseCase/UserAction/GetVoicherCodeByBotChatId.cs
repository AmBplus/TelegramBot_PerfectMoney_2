using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.UseCase.Notify;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.UserAction
{
    public record GetVoicherCodeByBotChatIdRequest(long UserBotChatId) : IRequest<ResultOperation<List<VoicherCodeDto>>>
    {

    }

    public class VoicherCodeDto
    {
        public string VoicherCode { get; set; }

        public long UserBotChatId { get; set; }
        
        public long OrderId { get; set; }
    }

    public class GetVoicherCodeByBotChatIdHandler : IRequestHandler<GetVoicherCodeByBotChatIdRequest, ResultOperation<List<VoicherCodeDto>>>
    {
        public GetVoicherCodeByBotChatIdHandler(ITelContext context , ILogger<GetVoicherCodeByBotChatIdHandler> logger ,IMediator mediator )
        {
            Context = context;
            Logger = logger;
            Mediator = mediator;
        }

        public ITelContext Context { get; }
        public ILogger<GetVoicherCodeByBotChatIdHandler> Logger { get; }
        public IMediator Mediator { get; }

        public async Task<ResultOperation<List<VoicherCodeDto>>> Handle(GetVoicherCodeByBotChatIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = Context.VoicherCodes.Where(x => x.UserBotChatId == request.UserBotChatId)
            .Select(x => new VoicherCodeDto
            { OrderId = x.OrderId, UserBotChatId = x.UserBotChatId, VoicherCode = x.VoicherCode }).ToList();
                if (result == null || result.Count == 0)
                {
                    return ResultOperation<List<VoicherCodeDto>>.ToFailedResult("نتیجه ای پیدا نشد");
                }
                return result.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.InnerException?.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}--{e.InnerException?.Message}"));
                return ResultOperation<List<VoicherCodeDto>>.ToFailedResult("مشکلی پیش آمده ، در اسرع وقت به مشکل رسیدگی خواهد شد ، شکیبا باشید.");
            }

        }
    }
}
