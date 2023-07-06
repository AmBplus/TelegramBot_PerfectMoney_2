using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.UseCase.Notify;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.UserAction
{
    public record GetUserByBotUserIdQueryRequest : IRequest<UserDto?>
    {
        public long UserId { get; set; }

        public GetUserByBotUserIdQueryRequest(long userId)
        {
            UserId = userId;
        }
    }


    public class GetUserByBotUserIdQueryHandler : IRequestHandler<GetUserByBotUserIdQueryRequest, UserDto?>
    {
        ITelContext Context { get; set; }
        public IMediator Mediator { get; }
        public  ILogger<GetUserByBotUserIdQueryHandler> Logger { get; }

        public GetUserByBotUserIdQueryHandler(ITelContext context,IMediator mediator,
            ILogger<GetUserByBotUserIdQueryHandler> logger)
        {
            Context = context;
            Mediator = mediator;
            Logger = logger;
        }

        public async Task<UserDto?> Handle(GetUserByBotUserIdQueryRequest request, CancellationToken cancellationToken = default)
        {

            try
            {
                var result = await Context.Users.Include(x => x.Roles).Where(x => x.BotChatId == request.UserId).Select(x => new UserDto
                {
                    BotChatId= x.BotChatId,
                    PhoneNumber = x.PhoneNumber,
                    Id = x.Id,
                    Roles = new RoleDto
                    {
                        Id = x.Roles!.Id,
                        Name = x.Roles!.Name,
                    },
                    IsActive = x.Active
                }).FirstOrDefaultAsync(cancellationToken);
                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return null;
            }
          
        }
    }
}
