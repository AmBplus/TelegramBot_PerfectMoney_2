using MediatR;
using Microsoft.EntityFrameworkCore;
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
    public class GetUserByBotUserIdQueryRequest : IRequest<UserModel?>
    {
        public long UserId { get; set; }

        public GetUserByBotUserIdQueryRequest(long userId)
        {
            UserId = userId;
        }
    }
    public class GetUserByBotUserIdQueryHandler : IRequestHandler<GetUserByBotUserIdQueryRequest, UserModel?>
    {
        ITelContext Context { get; set; }

        public GetUserByBotUserIdQueryHandler(ITelContext context)
        {
            Context = context;
        }

        public async Task<UserModel?> Handle(GetUserByBotUserIdQueryRequest request, CancellationToken cancellationToken = default)
        {
            UserModel user;
            var result = await Context.Users.FirstOrDefaultAsync(x => x.BotUserId == request.UserId, cancellationToken);
            return result;
          
        }
    }
}
