﻿using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Shared.Utility.ResultUtil;
using Microsoft.EntityFrameworkCore;
using PefectMoney.Core.UseCase.Notify;

namespace PefectMoney.Core.UseCase.UserAction
{
    public record GetUserCardsRequest(long ChatId) : IRequest<ResultOperation<GetUserCardsResponse>>
    {
     
    }
    public record GetUserCardsResponse(long ChatId)
    {
     
        public IList<UserCardsDto> UserCards { get; set; } 
    }
    public class GetUserCardsHandler : IRequestHandler<GetUserCardsRequest,ResultOperation<GetUserCardsResponse>>
    {
        public GetUserCardsHandler(ITelContext context,ILogger<GetUserCardsHandler> logger,IMediator mediator)
        {
            Context = context;
            Logger = logger;
            Mediator = mediator;
        }

        public ITelContext Context { get; }
        public ILogger<GetUserCardsHandler> Logger { get; }
        public IMediator Mediator { get; }

        public async Task<ResultOperation<GetUserCardsResponse>> Handle(GetUserCardsRequest request, CancellationToken cancellationToken)
        {
            try
            {
              
                var result = Context.Users.Where(x => x.BotChatId == request.ChatId)
                    .Include(x => x.BankAccountNumbers)
                    .Select(x => new GetUserCardsResponse(x.BotChatId)
                {
                    UserCards = x.BankAccountNumbers
                   .Select(b =>
                     new UserCardsDto(b.CartNumber) { }).ToList(),

                }).FirstOrDefault();
              if(result==null)
                {
                     return ResultOperation<GetUserCardsResponse>.ToFailedResult("کاربر یافت نشد");
                }
                return result.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message,e.InnerException?.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation<GetUserCardsResponse>.ToFailedResult("مشکلی پیش آمده با ادمین تماس حاصل فرمایید");
            }
          
        }
    }
}
