﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.UseCase.Notify;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.UserAction
{
    public record AddCardCommandRequest : IRequest<ResultOperation>
    {
        public long BotUserId { get; set; }
        public string CardNumber { get; set;}
    }
    public class AddCardCommandHandler : IRequestHandler<AddCardCommandRequest, ResultOperation>
    {
        public AddCardCommandHandler(ITelContext context ,IMediator mediator,
            ILogger<AddCardCommandHandler> logger)
        {
            Context = context;
            Mediator = mediator;
            Logger = logger;
        }

        public ITelContext Context { get; }
        public IMediator Mediator { get; }
        public ILogger<AddCardCommandHandler> Logger { get; }

        public async Task<ResultOperation> Handle(AddCardCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await Context.Users.Include(c => c.BankAccountNumbers).FirstOrDefaultAsync(x => x.BotChatId == request.BotUserId);
                if (user == null)
                {
                    var message = $"کاربر با چت آیدی {request.BotUserId} در دیتا بیس یافت نشد";
                    Logger.LogInformation(message);
                    return ResultOperation.ToFailedResult(message);
                };
                BankCardEntity bankCard = new BankCardEntity() { UserId = user.Id, CartNumber = request.CardNumber };
                await Context.BankCards.AddAsync(bankCard);
                await Context.SaveChangesAsync();

                return ResultOperation.ToSuccessResult("کارت ثبت شد");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation.ToFailedResult("خطایی پیش آمده با ادمین تماس حاصل فرمایید");
            }
        }
    }
}
