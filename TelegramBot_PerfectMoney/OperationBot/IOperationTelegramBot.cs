using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot_PerfectMoney.OperationBot
{
    public interface IOperationTelegramBot
    {
        Task Start(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task AdminMainSection(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task GetContact(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task AdminUserListSection(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task BackToMainSection(ITelegramBotClient  botClient, Update update, CancellationToken cancellationToken);
        Task BackToPreviousnStep(ITelegramBotClient  botClient, Update update, CancellationToken cancellationToken);
        Task GetUserList(ITelegramBotClient botClient, Update update , CancellationToken cancellationToken, string page);
        // Task SendNumberRequest(ITelegramBotClient botClient,Update  update, CancellationToken cancellationToken);
        Task SearchUserByPhoneNumber(ITelegramBotClient botClient,Update  update, CancellationToken cancellationToken);
        Task ActiveUser(ITelegramBotClient botClient, Update update , CancellationToken cancellationToken);
        Task BlockUser(ITelegramBotClient botClient, Update update , CancellationToken cancellationToken);
        Task SendMessageToUser(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task SendMessageToAllUsers(ITelegramBotClient botClient,Update update,CancellationToken cancellationToken);
        Task ActivSelling(ITelegramBotClient botClient,Update update , CancellationToken cancellationToken);
        Task StopSelling(ITelegramBotClient botClient,Update update , CancellationToken cancellationToken);
        Task StopBot(ITelegramBotClient botClient,Update update ,CancellationToken cancellationToken);
        Task SaveContact(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task SendRuleTextToAdmin(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task SendRule(ITelegramBotClient botClient, Update update, CancellationToken cancellation);
        Task GetRuleText(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
    }
}
