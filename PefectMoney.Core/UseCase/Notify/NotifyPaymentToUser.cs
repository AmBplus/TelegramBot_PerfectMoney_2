using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Data;
using PefectMoney.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace PefectMoney.Core.UseCase.Notify
{
    public record NotifyPaymentToUserRequest : INotification
    {
        public long OrderId { get; set; }
        public long BotChatId { get; set; }
        public string Message { get; set; }   
    }
    public class NotifyPaymentToUserHandler : INotificationHandler<NotifyPaymentToUserRequest>
    {
        public NotifyPaymentToUserHandler(ITelegramBotClient botClient, ITelContext context,ILogger<NotifyPaymentToUserHandler> logger, IMediator mediator)
        {
            BotClient = botClient;
            Context = context;
            Logger = logger;
            Mediator = mediator;
        }

        public ITelegramBotClient BotClient { get; }
        public ITelContext Context { get; }
        public ILogger<NotifyPaymentToUserHandler> Logger { get; }
        public IMediator Mediator { get; }

        public async Task Handle(NotifyPaymentToUserRequest notification, CancellationToken cancellationToken)
        {
            try
            {
                var txt = StringExtensionHelper.CreateString($"آیدی چت بات کاربر : {notification.BotChatId} --- ",
                    $"شماره سفارش :{notification.OrderId}", $"پیام :{notification.Message}");
                await  BotClient.SendTextMessageAsync(chatId: notification.BotChatId, text: txt);
            }
            catch (Exception e)
            {
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}--{e.InnerException?.Message}"));
            }
        }
    }
}
