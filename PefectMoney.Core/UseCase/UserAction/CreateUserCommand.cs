using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.UserAction
{
    public record CreateUserCommandRequest : IRequest<ResultOperation>
    {
       public  UserModel UserModel { get; set; }

        public CreateUserCommandRequest(UserModel userModel)
        {
            UserModel = userModel;
        }
    }
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, ResultOperation>
    {
        ITelContext Context { get; set; }
        public ILogger<CreateUserCommandHandler> Logger { get; }

        public CreateUserCommandHandler(ITelContext context,ILogger<CreateUserCommandHandler> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<ResultOperation> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await Context.AddAsync(request.UserModel, cancellationToken);
                await Context.SaveChangesAsync();
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
