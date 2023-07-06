using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.UseCase.Notify;
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
       
        public UserDto UserDto { get; }

        public CreateUserCommandRequest(UserDto userDto)
        {
            
            UserDto = userDto;
        }
    }
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, ResultOperation>
    {
        ITelContext Context { get; set; }
        public IMediator Mediator { get; }
        public ILogger<CreateUserCommandHandler> Logger { get; }

        public CreateUserCommandHandler(ITelContext context,IMediator mediator,
            ILogger<CreateUserCommandHandler> logger)
        {
            Context = context;
            Mediator = mediator;
            Logger = logger;
        }

        public async Task<ResultOperation> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = new UserEntity(request.UserDto.BotChatId, request.UserDto.PhoneNumber, request.UserDto.Roles.Id);
                await Context.AddAsync(user, cancellationToken);
                await Context.SaveChangesAsync(cancellationToken);
                return ResultOperation.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation.ToFailedResult();
            }
        }
    }
}
