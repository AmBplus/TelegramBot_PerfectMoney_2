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
    public record ChangeUserStatusCommandRequest(long botChatId,bool Status) : IRequest<ResultOperation>
    {
    }
    public class ChangeUserStatusCommandHandler : IRequestHandler<ChangeUserStatusCommandRequest, ResultOperation>
    {
        public ChangeUserStatusCommandHandler(ITelContext Context , ILogger<ChangeUserStatusCommandHandler> logger)
        {
            this.Context = Context;
            Logger = logger;
        }

        public ITelContext Context { get; }
        public ILogger<ChangeUserStatusCommandHandler> Logger { get; }

        public async Task<ResultOperation> Handle(ChangeUserStatusCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await Context.Users.FirstOrDefaultAsync(x => x.BotChatId == request.botChatId, cancellationToken);
                if (user != null)
                {
                    user.Active = request.Status;
                    Context.Users.Update(user);
                }
                return ResultOperation.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return ResultOperation.ToFailedResult("خطایی پیش آمده");
            }
        }
    }
}
