using MediatR;
using PefectMoney.Core.Data;
using PefectMoney.Shared.Utility.ResultUtil;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.SymbolStore;

namespace PefectMoney.Core.UseCase.UserAction;

    public record IsUserExistRequest : IRequest<bool>
    {
    public long UserId { get; set; }

    public IsUserExistRequest(long userId)
    {
        UserId = userId;
    }
}
    public class IsUserExistQuery : IRequestHandler<IsUserExistRequest, bool>
    {
    public IsUserExistQuery(ITelContext context)
    {
        Context = context;
    }

    public ITelContext Context { get; }

    public async Task<bool> Handle(IsUserExistRequest request, CancellationToken cancellationToken = default)
    {
        var result = await Context.Users.AnyAsync(x => x.BotChatId == request.UserId,cancellationToken);
        return result;    
    }
}

