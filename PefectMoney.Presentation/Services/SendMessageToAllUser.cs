using PefectMoney.Core.UseCase.UserAction;
using System.Runtime.CompilerServices;
using Telegram.Bot;

namespace PefectMoney.Presentation.Services
{
    public static class SendMessageToAllUserRequest 
    {
        public static async Task SendMessageToAll(this ITelegramBotClient _botClient ,List<UserDto> users,string message,CancellationToken cancellationToken = default)
        {
            foreach (var user in users)
            {
                _botClient.SendTextMessageAsync(chatId: user.BotChatId, text: message,cancellationToken: cancellationToken);
            }
        }
    }
  

}
