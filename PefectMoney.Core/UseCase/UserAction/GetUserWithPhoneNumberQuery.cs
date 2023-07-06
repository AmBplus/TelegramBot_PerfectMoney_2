using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.UseCase.Notify;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.UserAction
{
    public class GetUserWithPhoneNumberQueryRequest : IRequest<ResultOperation<UserDto>>
    {
        public string PhoneNumber { get; set; }

        public GetUserWithPhoneNumberQueryRequest(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
    public class GetUserWithPhoneNumberQueryHandler : IRequestHandler<GetUserWithPhoneNumberQueryRequest, ResultOperation<UserDto>>
    {
        ITelContext Context { get; set; }
        public IMediator Mediator { get; }
        public ILogger<GetUserWithPhoneNumberQueryHandler> Logger { get; }

        public GetUserWithPhoneNumberQueryHandler(ITelContext context,IMediator mediator ,
            ILogger<GetUserWithPhoneNumberQueryHandler> logger)
        {
            Context = context;
            Mediator = mediator;
            Logger = logger;
        }

        public async Task<ResultOperation<UserDto>> Handle(GetUserWithPhoneNumberQueryRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
              var user = await Context.Users
                    .Where(x => x.PhoneNumber!.Contains(request.PhoneNumber))
                    .Select(x=> new UserDto
                    {
                        BotChatId = x.BotChatId,
                        PhoneNumber = x.PhoneNumber,
                        Id = x.Id,
                        Roles = new RoleDto
                        {
                            Id = x.Roles!.Id,
                            Name = x.Roles!.Name,
                           
                        },
                        IsActive = x.Active
                        

                    })
                    .FirstOrDefaultAsync(cancellationToken);
                if (user == null)
                {
                    return ResultOperation<UserDto>.ToFailedResult();
                }
                return user.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.InnerException?.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation<UserDto>.ToFailedResult();
            }
          
        }
    }
}
