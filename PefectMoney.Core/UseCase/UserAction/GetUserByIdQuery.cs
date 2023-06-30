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
    public class GetUserByIdQueryRequest : IRequest<UserModel?>
    {
        public long UserId { get; set; }

        public GetUserByIdQueryRequest(long userId)
        {
            UserId = userId;
        }
    }
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQueryRequest, UserModel?>
    {
        ITelContext Context { get; set; }

        public GetUserByIdQueryHandler(ITelContext context)
        {
            Context = context;
        }

        public async Task<UserModel?> Handle(GetUserByIdQueryRequest request, CancellationToken cancellationToken)
        {
            UserModel user;
            var result = await Context.Users.FirstOrDefaultAsync(x => x.UserId == request.UserId);
            return result;
          
        }
    }
}
