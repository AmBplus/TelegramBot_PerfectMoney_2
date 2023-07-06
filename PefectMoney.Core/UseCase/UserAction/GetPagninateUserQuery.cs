using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Shared.Utility.ResultUtil;
using PefectMoney.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Security.Cryptography.X509Certificates;
using PefectMoney.Core.UseCase.Notify;

namespace PefectMoney.Core.UseCase.UserAction
{
    public class GetPagninateUserQueryRequest : IRequest<ResultOperation<PaginateGetUserQueryResponse>>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

    }
    public record PaginateGetUserQueryResponse
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public bool IsLast { get; set; }
        public long TotalRecord { get; set; }
        public List<UserDto> users { get; set; }    
    }
    public class GetPagninateUserQueryHandler : IRequestHandler<GetPagninateUserQueryRequest, ResultOperation<PaginateGetUserQueryResponse>>
    {
        public GetPagninateUserQueryHandler(ITelContext context ,IMediator mediator,
            ILogger<GetUserByBotUserIdQueryHandler> logger )
        {
            Context = context;
            Mediator = mediator;
            Logger = logger;
        }

        public ITelContext Context { get; }
        public IMediator Mediator { get; }
        public ILogger<GetUserByBotUserIdQueryHandler> Logger { get; }

        public async Task<ResultOperation<PaginateGetUserQueryResponse>> Handle(GetPagninateUserQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                List<UserDto> listUserDto;
                long total = 0;
                if (request.PageSize == 0)
                {
                    listUserDto = await Context.Users.Select(x => new UserDto()
                    {
                        BotChatId = x.BotChatId,
                        Id = x.Id,
                        PhoneNumber = x.PhoneNumber,
                        Roles = new RoleDto
                        {
                            Id = x.RoleId,
                            Name = x.Roles!.Name,

                        },
                        IsActive = x.Active,
                        Cards = x.BankAccountNumbers.Select(c=> new UserCardsDto(c.CartNumber)
                        {
                            Id = c.Id
                        })
                    }).ToListAsync();
                }
                else
                {

                    listUserDto = listUserDto = await Context.Users.ToPaged(request.PageNumber, request.PageSize, out total).Select(x => new UserDto()
                    {
                        BotChatId = x.BotChatId,
                        Id = x.Id,
                        PhoneNumber = x.PhoneNumber,
                        Roles = new RoleDto
                        {
                            Id = x.RoleId,
                            Name = x.Roles.Name,

                        },
                        IsActive = x.Active,
                        Cards = x.BankAccountNumbers.Select(c => new UserCardsDto(c.CartNumber)
                        {
                            Id = c.Id
                        })
                    }).ToListAsync();
                }

                PaginateGetUserQueryResponse response = new PaginateGetUserQueryResponse()
                {
                    IsLast = listUserDto.Count < request.PageSize ,
                    PageNumber = request.PageNumber ,
                    PageSize = request.PageSize ,   
                    TotalRecord = total,
                    users = listUserDto
                }; 
                if(listUserDto == null)
                {
                    Logger.LogError("تعداد کاربران دریافتی در این تابع نمیتواند نال بشود {GetPagninateUserQueryHandler}");
                    await Mediator.Publish(new NotifyAdminRequest($"{GetPagninateUserQueryHandler} تعداد کاربران دریافتی در این تابع نمیتواند نال بشود"));
                    return ResultOperation<PaginateGetUserQueryResponse>.ToFailedResult("خطایی پیش آمده");
                }
                return response.ToSuccessResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation<PaginateGetUserQueryResponse>.ToFailedResult("تعداد کاربران نمیتواند نال بشود");
            }

       
        }
    }
}
