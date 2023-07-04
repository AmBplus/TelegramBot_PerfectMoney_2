using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.UserAction
{
    public record GetAllActiveUserChatIdRequest() : IRequest<ResultOperation<List<UserDto>>>
    {
    }
    public class GetAllActiveUserChatIdHandler : IRequestHandler<GetAllActiveUserChatIdRequest, ResultOperation<List<UserDto>>>
    {
        ITelContext Context { get; set; }
        public ILogger<GetAllActiveUserChatIdHandler> Logger { get; }

        public GetAllActiveUserChatIdHandler(ITelContext context , ILogger<GetAllActiveUserChatIdHandler> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<ResultOperation<List<UserDto>>> Handle(GetAllActiveUserChatIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await Context.Users.Where(x => x.Active).Select(x => new UserDto
                {
                    BotChatId = x.BotChatId,
                    IsActive = x.Active
                }).ToListAsync(cancellationToken);
                return result.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e); ;
                return ResultOperation< List < UserDto >>.ToFailedResult();
            }
        
        }
    }
}
