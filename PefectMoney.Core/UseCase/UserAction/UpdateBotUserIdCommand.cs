using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.UserAction
{
    public class UpdateBotUserIdRequest  : IRequest<ResultOperation>
    {
        public long UserModelId { get; }

        public long BotUserId { get; set; }

    

        public UpdateBotUserIdRequest(long userModelId, long botUserId)
        {
            UserModelId = userModelId;
            BotUserId = botUserId;
        }
    }
    public class UpdateBotUserIdHandler : IRequestHandler<UpdateBotUserIdRequest, ResultOperation>
    {
        public UpdateBotUserIdHandler(ILogger<UpdateBotUserIdHandler> logger , ITelContext context )
        {
            Logger = logger;
            Context = context;
        }

        public ILogger<UpdateBotUserIdHandler> Logger { get; }
        public ITelContext Context { get; }

        public async Task<ResultOperation> Handle(UpdateBotUserIdRequest request, CancellationToken cancellationToken = default )
        {
            try
            {
                var user =  await Context.Users.FirstOrDefaultAsync(x=>x.Id == request.UserModelId, cancellationToken);
                if (user == null)
                {
                    Logger.LogError("کاربر ارسالی وجود خارجی در دیتابیس ندارد!");
                    return ResultOperation.ToFailedResult();
               
                }
                user.BotChatId = request.BotUserId;
                await Context.SaveChangesAsync(cancellationToken);
                return ResultOperation.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                return ResultOperation.ToFailedResult();
            }
        }
    }
}
