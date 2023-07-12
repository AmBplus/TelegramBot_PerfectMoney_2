using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace PefectMoney.Core.UseCase.Notify
{
    public record NotifyAdminRequest (string Message): INotification
    {
        
    }
    public class NotifyAdminsHandler : INotificationHandler<NotifyAdminRequest>
    {
        public NotifyAdminsHandler(ITelegramBotClient botClient , ITelContext context, ILogger<NotifyAdminsHandler> logger)
        {
            BotClient = botClient;
            Context = context;
            Logger = logger;
         
        }

        public ITelegramBotClient BotClient { get; }
        public ITelContext Context { get; }
        public ILogger<NotifyAdminsHandler> Logger { get; }

        public async Task Handle(NotifyAdminRequest notification, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Log To Admin ");
            try
            {
                var adminUsers = await Context.Users.Where(x => x.RoleId == RoleName.Admin).ToListAsync();
                foreach (var adminUser in adminUsers)
                {
                    try
                    {
                        if(adminUser.BotChatId !=null)
                        {
                            await BotClient.SendTextMessageAsync(chatId: adminUser.BotChatId, text: $"نوتیفیکشن : {notification.Message}");
                        }
                        Logger.LogInformation("----- Admin Don't Have BotChatID -----");
                    }
                    catch (Exception e)
                    {

                        Logger.LogCritical(e.Message, e.InnerException?.Message);
                        
                    }
                }
            }
         
            catch (Exception e)
            {
                Logger.LogCritical(e.Message, e.InnerException?.Message);
               

            }
         
        }
    }
}
