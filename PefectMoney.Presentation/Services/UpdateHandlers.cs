using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using PefectMoney.Presentation.Services;
using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using PefectMoney.Presentation.PresentationHelper.OperationBot;
using PefectMoney.Core.Data;
using PefectMoney.Presentation.PresentationHelper;
using PefectMoney.Core.Model;

using MediatR;
using PefectMoney.Core.UseCase.UserAction;
using System.Threading;
using PefectMoney.Core.Settings;
using PefectMoney.Core.UseCase._BotSettings;
using PefectMoney.Core.Extensions;
using System.Text;

namespace PefectMoney.Presentation.Services;


public class UpdateHandlers
{
   
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandlers> _logger;
    public IMediator MediatR { get; }


    public UpdateHandlers(ITelegramBotClient botClient,IMediator mediatR, ILogger<UpdateHandlers> logger)
    {
        _botClient = botClient;
        MediatR = mediatR;
       
        _logger = logger;
    }

    

#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
    public Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _                                       => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }
    
    public async Task HandleUpdateAsync(Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {

    if(update.CallbackQuery !=null)
        {
            update.Message = update.CallbackQuery.Message;
            update.Message.Text = update.CallbackQuery.Data;
        }
  
    var handler = update switch
        {
         
            { Message: { } message }                       => BotOnMessageReceived(message, cancellationToken),   
            _                                              => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }
    
    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
   
        var user = await MediatR.Send(new GetUserByBotUserIdQueryRequest(message.Chat.Id) );
        if(user == null)
        {
            var result = await CreateUser(message,cancellationToken);
            if (result == null) return;
            user = result;
        }
        if (message.Text is not { } messageText)
            return;
        ActionHelper.BotActions.TryGetValue(message.Chat.Id, out BotAction botAction);

        if(botAction != null)
        {
            await  HandleActions(_botClient, message,botAction,user, cancellationToken);
            return;
        }
       


        if (user.Roles.Id == RoleName.Admin)
        {

            var action = messageText switch
            {

                BotNameHelper.Law => ShowLaws(_botClient, message, cancellationToken),
                BotNameHelper.AboutUs => AboutUs(_botClient, message, cancellationToken),
                BotNameHelper.BuyingProduct => BuyingProduct(_botClient, message, cancellationToken),
                BotNameHelper.Cards => Cards(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel => AdminMenu(_botClient, message, cancellationToken),
                _ => AdminMenu(_botClient, message, cancellationToken)
            };

            Message sentMessage = await action;
            _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
        }
        else // User 
        {
          
            var action = messageText switch
            {

                BotNameHelper.Law => ShowLaws(_botClient, message, cancellationToken),
                BotNameHelper.AboutUs => AboutUs(_botClient, message, cancellationToken),
                BotNameHelper.BuyingProduct => BuyingProduct(_botClient, message, cancellationToken),
                BotNameHelper.Cards => Cards(_botClient, message, cancellationToken),
                BotNameHelper.AddNewCard => AddNewCard(_botClient, message, cancellationToken),
                BotNameHelper.RegisteredCards => RegisteredCards(_botClient, message, cancellationToken),
                BotNameHelper.Menu => UserMenu(_botClient, message, cancellationToken),
                _ => UserMenu(_botClient, message, cancellationToken)
            };

            Message sentMessage = await action;
            _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
        }
    

        static async Task<Message> UserMenu(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Menu",
                replyMarkup: CreatKeyboardHelper.GetUserMenuKeyBoard(),
                cancellationToken: cancellationToken);
        }
    }

    private async Task HandleActions(ITelegramBotClient botClient, Message message,BotAction botAction,UserDto userDto, CancellationToken cancellationToken)
    {
        switch (botAction.ActionName)
        {
            case BotNameHelper.SeeRegisteredCards :
            {
                    await SeeRegisteredCards(botClient, message, cancellationToken);
                    return;
            }
            

            default: break;
        }

    }

    private async Task<Message> SeeRegisteredCards(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {

        var userResult = await MediatR.Send(new GetUserCardsRequest(message.Chat.Id));
        string text;
        if (userResult.IsSuccess == false)
            text = userResult.Message!.ToStringEnumerable();
        else
        {
            if (userResult.Data.UserCards == null)
            {
                text = "کارتی ثبت نشده";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in userResult.Data!.UserCards)
                {
                    sb.AppendLine($"شماره کارت ثبت شده : {item.CardNumber}");
                }
                text = sb.ToString();
            }
        }
        ActionHelper.BotActions.Remove(message.Chat.Id);
        return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: text,
               replyMarkup: new ReplyKeyboardRemove(),
               cancellationToken: cancellationToken);

    }
    private async Task<Message> RegisteredCards(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {

            ActionHelper.BotActions.Add(message.Chat.Id, new BotAction { ActionName = BotNameHelper.SeeRegisteredCards, ActionStatus = ActionStatus.OnProccess });
        
            return await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: "کارت خود را وارد نمایید",
                   replyMarkup: new ReplyKeyboardRemove(),
                   cancellationToken: cancellationToken);
    
    }

    private async Task<Message> AddNewCard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        
        ActionHelper.BotActions.Add(message.Chat.Id, new BotAction { ActionName = BotNameHelper.AddNewCard, ActionStatus = ActionStatus.OnProccess });
        if(ActionHelper.BotActions.ContainsKey(message.Chat.Id))
        {

        }
        return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: "کارت خود را وارد نمایید",
               replyMarkup: new ReplyKeyboardRemove(),
               cancellationToken: cancellationToken);
   
    }
    private async Task<Message> NewCard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {

        ActionHelper.BotActions.Add(message.Chat.Id, new BotAction { ActionName = BotNameHelper.AddNewCard, ActionStatus = ActionStatus.OnProccess });
        if (ActionHelper.BotActions.ContainsKey(message.Chat.Id))
        {

        }
        return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: "کارت خود را وارد نمایید",
               replyMarkup: new ReplyKeyboardRemove(),
               cancellationToken: cancellationToken);

    }

    private async Task<Message> AdminMenu(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Menu",
            replyMarkup: CreatKeyboardHelper.GetUserMenuKeyBoard(),
            cancellationToken: cancellationToken);
    }

    private async Task<Message> Cards(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        return await botClient.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 text: "منو کارت ها",
                 replyMarkup: CreatKeyboardHelper.GetCardsMenuKeyBoard(),
                 cancellationToken: cancellationToken);
    }

    private Task<Message> BuyingProduct(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<Message> AboutUs(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var settings = await MediatR.Send(new GetBotSettingsRequest(), cancellationToken);
        return await botClient.SendTextMessageAsync(
           chatId: message.Chat.Id,
           text: settings.AboutUs,
           replyMarkup: CreatKeyboardHelper.GetUserMenuKeyBoard(),
           cancellationToken: cancellationToken);
    }

    private  async Task<Message> ShowLaws(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var settings = await MediatR.Send(new GetBotSettingsRequest(), cancellationToken);
        return await botClient.SendTextMessageAsync(
           chatId: message.Chat.Id,
           text: settings.RuleTextAsOneString,
           replyMarkup: CreatKeyboardHelper.GetUserMenuKeyBoard(),
           cancellationToken: cancellationToken);
    }

    private async Task<Message> AdminPanel(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<UserDto> CreateUser(Message message,CancellationToken cancellationToken)
    {
        var number = message.Contact?.PhoneNumber;
        
        if (number == null)
        {
            var ShareContactKeyboard = CreatKeyboardHelper.GetContactKeyboard();
            await _botClient.SendTextMessageAsync(
               chatId: message!.Chat.Id,
               text: "لطفا شماره تلفن خود را ارسال کنید\n بدون اشتراک شماره تلفن خود امکان استفاده از بات را ندارید.",
               replyMarkup: ShareContactKeyboard,
               cancellationToken: cancellationToken);
            ;
            return null;
        }
     // Check There is a User With This Phone Number
        var resultExistUserWithThisPhone = await MediatR.Send(new GetUserWithPhoneNumberQueryRequest(number));

        if(resultExistUserWithThisPhone.IsSuccess)
        {
            // Update User And Send It Back
         var resultUpdateBotUserId = await MediatR.Send(new UpdateBotUserIdRequest(resultExistUserWithThisPhone.Data.Id, message.Chat.Id));
           if(resultUpdateBotUserId.IsSuccess)
            {
                message.Text = BotNameHelper.Menu;
                return resultExistUserWithThisPhone.Data;
            }
            return null;
        }

        // Create A User
        var roleDto =  new RoleDto() { Id = RoleName.Customer };
        UserDto? newUser = new UserDto() { BotChatId = message.Chat.Id,PhoneNumber =  number,Roles = roleDto };

        var result = await MediatR.Send(new CreateUserCommandRequest(newUser));
                
        if(result == null)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "مشکلی پیش آمده با ادمین تماس حاصل فرمایید");
        }

        if(!result!.IsSuccess)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "مشکلی پیش آمده با ادمین تماس حاصل فرمایید");
            return null;
        }

        await _botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "به ربات خرید پرفکت مانی خوش آمدید",
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
        message.Text = BotNameHelper.Menu;
        return newUser;
    }

    private async Task<Message> StartCard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
       

        // Create an inline keyboard markup with the button
      
        
        // Send a message with the inline keyboard
       
        return await botClient.SendTextMessageAsync(message.Chat.Id, "شماره کارت خود را وارد نمایید");


    }

    private async Task<Message> StartSwitchtoInlinebuttons(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        // using Telegram.Bot.Types.ReplyMarkups;

        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
    InlineKeyboardButton.WithSwitchInlineQuery(
        text: "switch_inline_query"),
    InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(
        text: "switch_inline_query_current_chat"),
});   

        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "A message with an inline keyboard markup",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);

    }

    private async Task<Message> StartURLbuttons(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
    InlineKeyboardButton.WithUrl(
        text: "Link to the Repository",
        url: "https://github.com/TelegramBots/Telegram.Bot")
        });
        return await botClient.SendTextMessageAsync(
        chatId: message.Chat.Id,
        text: "A message with an inline keyboard markup",
        replyMarkup: inlineKeyboard,
        cancellationToken: cancellationToken);
        
    }


#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
    private Task UnknownUpdateHandlerAsync(Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
