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

namespace PefectMoney.Presentation.Services;


public class UpdateHandlers
{
   
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandlers> _logger;

   

    public UpdateHandlers(ITelegramBotClient botClient,IMediator mediatR, ILogger<UpdateHandlers> logger)
    {
        _botClient = botClient;
        MediatR = mediatR;
       
        _logger = logger;
    }

    public IMediator MediatR { get; }

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

    
  
    var handler = update switch
        {
         
            { Message: { } message }                       => BotOnMessageReceived(message, cancellationToken),   
            _                                              => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }
    private async Task ShareNumber()
    {

    }
    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
        if (message.Text is not { } messageText)
            return;
        var isUserExist = await MediatR.Send(new IsUserExistRequest(message.Chat.Id) );
        if(!isUserExist)
        {
            var result = await CreateUser(message,cancellationToken);
            if (!result) return;
        }
   

        //else if (userWithRoles.Roles.Role == RoleName.Admin.ToString())
        //{
        //    if (userWithRoles.ChatId == null)
        //    {
        //        userWithRoles.ChatId = message.Chat.Id.ToString();
        //        TelContext.Update(userWithRoles);
        //        TelContext.SaveChanges();
        //    }
        //    var mainKeyboardMarkup = CreatKeyboard.SetMainKeyboardMarkupForAdmin();
        //    UserStepHandler.DeleteAll(message.Chat.Id.ToString());
        //     await _botClient.SendTextMessageAsync(
        //        chatId: message!.Chat.Id,
        //        text: "به بات خرید پرفکت مانی خوش آمدید.",
        //        replyMarkup: mainKeyboardMarkup,
        //        cancellationToken: cancellationToken);
        //    ;
        //    return;
        //}
        //else
        //{
        //    var mainKeyboardMarkup = CreatKeyboard.SetMainKeyboardMarkupForUser();
        //    UserStepHandler.DeleteAll(message.Chat.Id.ToString());
        //     await _botClient.SendTextMessageAsync(
        //        chatId: message!.Chat.Id,
        //        text: "به بات خرید پرفکت مانی خوش آمدید.",
        //        replyMarkup: mainKeyboardMarkup,
        //        cancellationToken: cancellationToken);
        //}


        var action = messageText.Split(' ')[0] switch
        {
            "/inline_keyboard" => SendInlineKeyboard(_botClient, message, cancellationToken),
            "/keyboard" => SendReplyKeyboard(_botClient, message, cancellationToken),
            "/remove" => RemoveKeyboard(_botClient, message, cancellationToken),
            "/photo" => SendFile(_botClient, message, cancellationToken),
            "/request" => RequestContactAndLocation(_botClient, message, cancellationToken),
            "/inline_mode" => StartInlineQuery(_botClient, message, cancellationToken),
            "/sendLink" => StartURLbuttons(_botClient, message, cancellationToken),
            "/SendCard" => StartCard(_botClient, message, cancellationToken),
            "/SwitchtoInlinebuttons" => StartSwitchtoInlinebuttons(_botClient, message, cancellationToken),
            _ => Usage(_botClient, message, cancellationToken)
        };
        Message sentMessage = await action;
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

        // Send inline keyboard
        // You can process responses in BotOnCallbackQueryReceived handler
        static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendChatActionAsync(
                chatId: message.Chat.Id,
                chatAction: ChatAction.Typing,
                cancellationToken: cancellationToken);

            // Simulate longer running task
            await Task.Delay(500, cancellationToken);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    },
                });

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Removing keyboard",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        static async Task<Message> SendFile(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendChatActionAsync(
                message.Chat.Id,
                ChatAction.UploadPhoto,
                cancellationToken: cancellationToken);

            const string filePath = "Files/tux.png";
            await using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

            return await botClient.SendPhotoAsync(
                chatId: message.Chat.Id,
                photo: new InputFileStream(fileStream, fileName),
                caption: "Nice Picture",
                cancellationToken: cancellationToken);
        }

        static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            ReplyKeyboardMarkup RequestReplyKeyboard = new(
                new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Who or Where are you?",
                replyMarkup: RequestReplyKeyboard,
                cancellationToken: cancellationToken);
        }

        static async Task<Message> Usage(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            const string usage = "MENU:\n" +
                                 "/inline_keyboard - send inline keyboard\n" +
                                 "/keyboard    - send custom keyboard\n" +
                                 "/remove      - remove custom keyboard\n" +
                                 "/photo       - send a photo\n" +
                                 "/request     - request location or contact\n" +
                                 "/inline_mode - send keyboard with Inline Query\n"+
                                 "/SwitchtoInlinebuttons - SwitchtoInlinebuttons\n" +
                                 "/SendCard - SendCard\n" +
                                 "/sendLink - send githublink";

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: usage,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        static async Task<Message> StartInlineQuery(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new(
                InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode"));

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Press the button to start Inline Query",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
    }

    private async Task<bool> CreateUser(Message message,CancellationToken cancellationToken)
    {
        var number = message.Contact?.PhoneNumber;
        
        if (number == null)
        {
            var ShareContactKeyboard = CreatKeyboard.GetContactKeyboard();
            await _botClient.SendTextMessageAsync(
               chatId: message!.Chat.Id,
               text: "لطفا شماره تلفن خود را ارسال کنید\n بدون اشتراک شماره تلفن خود امکان استفاده از بات را ندارید.",
               replyMarkup: ShareContactKeyboard,
               cancellationToken: cancellationToken);
            ;
            return false;
        }
     
        UserModel? userWithRoles = null;


        var result = await MediatR.Send(new CreateUserCommandRequest(new UserModel(message.Chat.Id,number,(long)RoleName.Customer) { }));
                
        if(result == null)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "مشکلی پیش آمده با ادمین تماس حاصل فرمایید");
        }

        if(!result!.IsSuccess)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "مشکلی پیش آمده با ادمین تماس حاصل فرمایید");
            return false;
        }

        await _botClient.SendTextMessageAsync(message.Chat.Id, "به ربات خرید پرفکت مانی خوش آمدید");



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
