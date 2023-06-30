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
    public class GetUserWithPhoneNumberQueryRequest : IRequest<ResultOperation<UserModel>>
    {
        public string PhoneNumber { get; set; }

        public GetUserWithPhoneNumberQueryRequest(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
    public class GetUserWithPhoneNumberQueryHandler : IRequestHandler<GetUserWithPhoneNumberQueryRequest, ResultOperation<UserModel>>
    {
        ITelContext Context { get; set; }
        public ILogger<GetUserWithPhoneNumberQueryHandler> Logger { get; }

        public GetUserWithPhoneNumberQueryHandler(ITelContext context,ILogger<GetUserWithPhoneNumberQueryHandler> logger)
        {
            Context = context;
            Logger = logger;
        }

        public async Task<ResultOperation<UserModel>> Handle(GetUserWithPhoneNumberQueryRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
              var user = await Context.Users.FirstOrDefaultAsync(x => x.PhoneNumber!.Contains(request.PhoneNumber),cancellationToken);
                if (user == null)
                {
                    return ResultOperation<UserModel>.ToFailedResult();
                }
                return user.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                return ResultOperation<UserModel>.ToFailedResult();
            }
          
        }
    }
}
