using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using PefectMoney.Presentation.PresentationHelper.OperationBot;
using PefectMoney.Presentation.PresentationHelper;
using PefectMoney.Core.Model;
using MediatR;
using PefectMoney.Core.UseCase.UserAction;
using PefectMoney.Core.UseCase._BotSettings;
using PefectMoney.Core.Extensions;
using System.Text;
using PefectMoney.Presentation.PresentationHelper.Validator;
using Microsoft.Extensions.Options;
using PefectMoney.Core.Settings;
using PefectMoney.Core.UseCase.VerifyCard;
using PefectMoney.Core.UseCase._Shop;


namespace PefectMoney.Presentation.Services;


public class UpdateHandlers
{
   
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandlers> _logger;
    public IMediator MediatR { get; }
    public BotSettings BotSettings { get; }
    public Message patientMessage;

    public UpdateHandlers(ITelegramBotClient botClient,IMediator mediatR, ILogger<UpdateHandlers> logger , IOptions<BotSettings> options)
    {
        _botClient = botClient;
        MediatR = mediatR;
       
        _logger = logger;
        BotSettings = options.Value;
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
    
    private async Task RemoveAllPermantActions(long chatId)
    {
        await RemoveFromActionHelper(chatId);
        await RemovePagination(chatId);
    }

    public async Task HandleUpdateAsync(Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {

        Message messages;
        long id = 0;
        if (update.Message?.Chat?.Id != null)
        {
            id = update.Message!.Chat!.Id;
            messages = update.Message;
        }
        else if (update.CallbackQuery?.Message?.Chat.Id != null)
        {
            id = update.CallbackQuery.Message.Chat.Id;
            messages = update.CallbackQuery.Message;
        }
        else
        {
            await _botClient.SendTextMessageAsync(chatId: id, text: "نوع درخواست ارسالی تعریف نشده");
            return;
        }
         patientMessage = await _botClient.SendTextMessageAsync(id, "درخواست شما در حال بررسی است لطفا شکیبا باشید");
        #region User Get Or Create _ Update
        var user = await MediatR.Send(new GetUserByBotUserIdQueryRequest(id));
        if (user == null)
        {
            var result = await CreateUser(messages, cancellationToken);
            if (result == null) return;
            user = result;
        }
        #endregion
        var BotSettings = await MediatR.Send(new GetBotSettingsRequest());
        if(BotSettings.StopBot)
        {
            
            if(user == null ||  user.Roles.Id != RoleName.Admin)
            {
               await RemoveAllPermantActions(id);
               await _botClient.SendTextMessageAsync(chatId: id, text: BotNameHelper.AdminPanel_StopBot);
                await _botClient.DeleteMessageAsync(id, messageId: patientMessage.MessageId, cancellationToken);
                return;
            }

            await HandleStatusStopBotRequest(update,user,cancellationToken);
            await _botClient.DeleteMessageAsync(id, messageId: patientMessage.MessageId, cancellationToken);
            return;
        }
        

        var handler = update switch
        {
         
            { Message: { } message }                       => BotOnMessageReceived(message,user, cancellationToken),   
            { CallbackQuery: { } callbackQuery}            => BotOnCallBackReceived(callbackQuery,user, cancellationToken),   
            _                                              => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;

        
        await _botClient.DeleteMessageAsync(id, messageId: patientMessage.MessageId, cancellationToken);
        
    }

    private async Task HandleStatusStopBotRequest(Telegram.Bot.Types.Update update, UserDto user, CancellationToken cancellationToken)
    {
        if(update.CallbackQuery == null)
        {
            AddActionToActionHelper(update.Message!.Chat.Id,new BotAction() { ActionName = BotNameHelper.AdminPanel_StartBot});
            var textMessage = CreateString("در حال حاظر ربات در دست تعمیر است", "اگر قصد خارج کردن از این وضعیت را دارید", "تایید:شروع دوباره ربات", "کنسل:در همین وضعیت بماند");
            await _botClient.SendTextMessageAsync(chatId: update.Message!.Chat.Id, text: textMessage,replyMarkup:CreateKeyboardHelper.Get_Accept_UnAccept_Menu_MenuKeyBoard());
            return;
        }
        if (update.CallbackQuery.Data != BotNameHelper.AcceptAction) {
            await RemoveAllPermantActions(update.CallbackQuery.Message.Chat.Id);
            await _botClient.SendTextMessageAsync(chatId: update.Message!.Chat.Id, text: "منو", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        }
        await StartBot(update.CallbackQuery.Message.Chat.Id);
    }

    private async Task BotOnCallBackReceived(CallbackQuery callback,UserDto user, CancellationToken cancellationToken)
    {
      
        if(callback == null || callback.Message== null)
        {
            return;
        }

      
        if(!user.IsActive)
        {
           await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "دسترسی شما این بات گرفته شده و اجازه دسترسی ندارید");
            return;
        }
        
        if (callback.Data is not { } callbackData)
            return;
        if (BotSettings.StopBot && user.Roles.Id  != RoleName.Admin)
        {
            await RemoveAllPermantActions(user.BotChatId);
            await _botClient.DeleteMessageAsync(user.BotChatId, messageId: patientMessage.MessageId, cancellationToken);
            await _botClient.SendTextMessageAsync(chatId: user.BotChatId, text: "ربات در دست تعمیر است بعدا تلاش کنید");

            return;
        }

        ActionHelper.BotActions.TryGetValue(callback.Message.Chat.Id, out BotAction botAction);

        if (botAction == null) {
            await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "منو", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        } 

        if (callback.Data == BotNameHelper.CancelAction || callback.Data == BotNameHelper.AdminMenu || callback.Data == BotNameHelper.Menu ||
              callback.Data == BotNameHelper.SeeMenu || callback.Data == BotNameHelper.BackToMenu)
        {
            await RemoveAllPermantActions(callback.Message.Chat.Id);
           await _botClient.SendTextMessageAsync(chatId: user.BotChatId, text:"", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        }


        if (botAction.ActionName == BotNameHelper.Pagnination)
        {
            var action = callbackData switch
            {

                BotNameHelper.Pagnination_Number10 => PaginationQuery(10,_botClient,callback, botAction,user,cancellationToken),
                BotNameHelper.Pagnination_Number20 => PaginationQuery(20,_botClient,callback, botAction,user,cancellationToken),
                BotNameHelper.Pagnination_Number50 => PaginationQuery(50,_botClient, callback, botAction,user,cancellationToken),
                BotNameHelper.Pagnination_Number100 => PaginationQuery(100,_botClient, callback, botAction,user,cancellationToken),
                BotNameHelper.Pagnination_Number200 => PaginationQuery(200,_botClient, callback, botAction,user,cancellationToken),
                BotNameHelper.Pagnination_Number400 => PaginationQuery(400,_botClient, callback, botAction,user,cancellationToken),
                BotNameHelper.Pagnination_Number800 => PaginationQuery(800,_botClient, callback, botAction,user,cancellationToken),
                BotNameHelper.Pagination_AllNumber => PaginationQuery(0, _botClient, callback, botAction, user, cancellationToken),
                BotNameHelper.Paginate_See_Next_Page => PaginateNextPage( _botClient, callback, botAction, user, cancellationToken),
                BotNameHelper.CancelAction => CancelPagination( _botClient, callback, botAction, user, cancellationToken),
                BotNameHelper.Menu => CancelPagination( _botClient, callback, botAction, user, cancellationToken),
                _ => PaginationQuery(10, _botClient, callback, botAction, user, cancellationToken)
            };;
            await action;
        }
        
        if(callback.Data == BotNameHelper.AcceptAction)
        {
         

            Task action = botAction.ActionName switch
            {
           
                BotNameHelper.AddNewCard => DoAddNewCard(botAction,user, callback, cancellationToken),
                BotNameHelper.BuyVoicher => DoBuyVoicher(botAction,user, callback, cancellationToken),
                
                _ => CheckAdminActions(botAction,user, callback, cancellationToken),
            };

            await action;
        }
        
        
        return;

    
    }

    private async Task DoBuyVoicher(BotAction botAction, UserDto user, CallbackQuery callback, CancellationToken cancellationToken)
    {
        if(botAction.Count<=0)
        {
            await RemoveAllPermantActions(user.BotChatId);
            await _botClient.SendTextMessageAsync(chatId: user.Id,
                text: "تعداد خرید ووچر نا معتبر است",
                replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        }
        
        var result = await MediatR.Send(new GetVoicherPaymentLinkRequest(botAction.Count, user.BotChatId));
        if(!result.IsSuccess)
        {
            await RemoveAllPermantActions(user.BotChatId);
            await _botClient.SendTextMessageAsync(chatId: user.Id,
                text: result.Message?.ToStringEnumerable() ?? "خطایی پیش آمده بعدا امتحان نمایید",
                replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        }
        await RemoveAllPermantActions(user.BotChatId);
        var msg = CreateString("لینک پرداخت ووچر برای شما ارسال شد",
           "لطفا فیلرشکن خود را خاموش کرده"
           , "سپس بر روی لینک کلیک کنید"
           , "پس پرداخت نتیجه از طریق ربات برای شما ارسال خواهد شد.",
           "در صورت انصراف از خرید", "میتوانید متن زیر را تایپ کنید"
           , BotNameHelper.BackToMenu);

        var payUrl = result.Data.ToString();
        InlineKeyboardMarkup inlineKeyboard = new(new[]
       {
    InlineKeyboardButton.WithUrl(
        text: "Link to the Repository",
        url: payUrl)
});
        
        _logger.LogInformation($".......inlineKeyboard Pay Created .......");
        Message sentMessage = await _botClient.SendTextMessageAsync(
            chatId: user.BotChatId,
            text: "لینک پرداخت",
            replyMarkup: inlineKeyboard,
            cancellationToken: cancellationToken);



       
        //_logger.LogInformation($"send payment with link {result.Data}");

    
        
        //await _botClient.SendTextMessageAsync(chatId: user.Id,
        //    text: "لینک پرداخت برای شما ارسال شد.", replyMarkup: inlineKeyboard);
    }

    private async Task<Task> CheckAdminActions(BotAction botAction, UserDto user, CallbackQuery callback, CancellationToken cancellationToken)
    {
        if (user.Roles.Id == RoleName.Admin)
        {
            return  botAction.ActionName switch
            {
                BotNameHelper.AdminPanel_StopBot => StopBot(callback.Message.Chat.Id),
                BotNameHelper.AdminPanel_StartBot => StartBot(callback.Message.Chat.Id),
                BotNameHelper.AdminPanel_StartSell => StartSell(callback.Message.Chat.Id),
                BotNameHelper.AdminPanel_StopSell => StopSel(callback.Message.Chat.Id),
                BotNameHelper.AdminPanel_BanUser => DoBanUserBot(botAction, callback, cancellationToken),
                BotNameHelper.AdminPanel_UnBanUser => DoUnBanUserBot(botAction, callback, cancellationToken),
                BotNameHelper.AdminPanel_SetLaws => DoSetLaws(botAction, callback, cancellationToken),
                
                _ => RemoveAllPermantActions(callback.Message.Chat.Id)
            };
        }
        return DoNothing();
    }

    private async Task StopSel(long id)
    {
        await RemoveAllPermantActions(id);
        var result = await MediatR.Send(new ChangeSellStatusRequest(true));
        if (result.IsSuccess)
        {
            await _botClient.SendTextMessageAsync(chatId: id, text: " فروش متوقف شد");
            return;
        }
        await _botClient.SendTextMessageAsync(chatId: id, text: "مشکلی پیش آمده");

    }

    private async Task StartSell(long id)
    {
        await RemoveAllPermantActions(id);
        var result =  await MediatR.Send(new ChangeSellStatusRequest(false));
        if(result.IsSuccess)
        {
          await  _botClient.SendTextMessageAsync(chatId: id, text: "ربات از حالت توقف فروش خارج شد");
            return;  
        }
        await _botClient.SendTextMessageAsync(chatId: id, text: "مشکلی پیش آمده");


    }

    private async Task DoAddNewCard(BotAction botAction,UserDto user, CallbackQuery callback, CancellationToken cancellationToken)
    {
        await RemoveAllPermantActions(user.BotChatId) ;
      
        var trackId = Guid.NewGuid().ToString();
        var resultVerifyCard = await MediatR.Send(new VerifyUserCardRequestDto { cartNumber = botAction.Message, phoneNumber = user.PhoneNumber, trackId = trackId });
        if (resultVerifyCard.IsSuccess)
        {
            var resultAddCard = await MediatR.Send(new AddCardCommandRequest() { BotUserId = user.BotChatId, CardNumber = botAction.Message });
            if (resultAddCard.IsSuccess)
            {
                await RemoveAllPermantActions(user.BotChatId);
                await RemoveFromActionHelper(chatId: user.BotChatId);
                 await _botClient.SendTextMessageAsync(
              chatId: user.BotChatId,
              text: "کارت شما ثبت شد",
              replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
              cancellationToken: cancellationToken);
                return;
            }
            else
            {
                await RemoveFromActionHelper(chatId: user.BotChatId);
                var msg = resultAddCard?.Message?.ToStringEnumerable() ;
                if(string.IsNullOrWhiteSpace(msg))
                {
                    msg = "کارت ثبت نشد";
                }
                 await _botClient.SendTextMessageAsync(
            chatId: user.BotChatId,
            text: msg,
            replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
            cancellationToken: cancellationToken);
                return;
            }


        }


        await RemoveFromActionHelper(chatId: user.BotChatId);
         await _botClient.SendTextMessageAsync(
               chatId: user.BotChatId,
               text: resultVerifyCard.Message!.ToStringEnumerable() ?? "خطا ، مشکلی آمده",
               replyMarkup: new ReplyKeyboardRemove(),
               cancellationToken: cancellationToken);
        return;
    }

    private async Task DoNothing() { }
    private async Task DoSetLaws(BotAction botAction, CallbackQuery callback, CancellationToken cancellationToken)
    {
       var result = await MediatR.Send(new SetLawCommandRequest(botAction.Message));
        await RemoveAllPermantActions(callback.Message.Chat.Id);  
        await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "قوانین تغیر پیدا کرد", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
    }

    private async Task DoUnBanUserBot(BotAction botAction, CallbackQuery callback, CancellationToken cancellationToken)
    {
        long botId = 0;
        try
        {
            botId = Convert.ToInt64(botAction.Message);
        }
        catch (Exception)
        {
            await RemoveAllPermantActions(callback!.Message!.Chat.Id);
            await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "مشکلی در فرمت آیدی کاربر پیش آمده", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        }

        var result = await MediatR.Send(new ChangeUserStatusCommandRequest(botId, true));
        if (!result.IsSuccess)
        {
            await RemoveAllPermantActions(callback!.Message!.Chat.Id);
            await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "مشکلی در تغیر وضعیت کاربر پیش آمده", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        }
        await RemoveAllPermantActions(callback!.Message!.Chat.Id);
        await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "کاربر در صورت وجود unban شد", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
    }

    private async Task DoBanUserBot(BotAction botAction,CallbackQuery callback, CancellationToken cancellationToken)
    {
        long botId = 0;
        try
        {
             botId = Convert.ToInt64(botAction.Message);
        }
        catch (Exception)
        {
           await RemoveAllPermantActions(callback!.Message!.Chat.Id);
           await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "مشکلی در فرمت آیدی کاربر پیش آمده", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        }

       var result = await MediatR.Send(new ChangeUserStatusCommandRequest(botId, false));
        if(!result.IsSuccess)
        {
            await RemoveAllPermantActions(callback!.Message!.Chat.Id);
            await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "مشکلی در تغیر وضعیت کاربر پیش آمده", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
            return;
        }
        await RemoveAllPermantActions(callback!.Message!.Chat.Id);
        await _botClient.SendTextMessageAsync(chatId: callback.Message.Chat.Id, text: "کاربر در صورت وجود بن شد", replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
    }

    private async Task StopBot(long chatId)
    {
        string msg = "";
        await RemoveAllPermantActions(chatId);
        var result = await MediatR.Send(new BotChangeStatusCommandRequest(true));
        if(result.IsSuccess)
        {
            msg = "ربات به حالت توقف در آمد";
        }
        else
        {
            msg = "مشکلی پیش آمده";
        }
       await _botClient.SendTextMessageAsync(chatId: chatId, text: msg, replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
    }
    private async Task StartBot(long chatId)
    {
        string msg = "";
        await RemoveAllPermantActions(chatId);
        var result = await MediatR.Send(new BotChangeStatusCommandRequest(false));
        if (result.IsSuccess)
        {
            msg = "ربات از حالت توقف بیرون آمد";
        }
        else
        {
            msg = "مشکلی پیش آمده";
        }
        await _botClient.SendTextMessageAsync(chatId: chatId, text: msg, replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
    }
    private async Task CancelPagination(ITelegramBotClient botClient, CallbackQuery callback, BotAction botAction, UserDto user, CancellationToken cancellationToken)
    {
   
        await RemovePagination(callback.Message.Chat.Id);
        await RemoveFromActionHelper(callback.Message.Chat.Id);
        await botClient.SendTextMessageAsync(
        chatId: callback.Message.Chat.Id,
        text: "منو",
        replyMarkup: CreateKeyboardHelper.GetAdminMenuKeyboard());
        return;

    }

    private async Task PaginateNextPage(ITelegramBotClient botClient, CallbackQuery callback, BotAction botAction, UserDto user, CancellationToken cancellationToken)
    {
        var userPagination = GetUserPaginationBot(callback.Message.Chat.Id);
        
        if (userPagination == null || userPagination.IsLast)
        {
            await RemoveFromActionHelper(callback.Message.Chat.Id);
            await botClient.SendTextMessageAsync(
            chatId: callback.Message.Chat.Id,
            text:"منو",
            replyMarkup: CreateKeyboardHelper.GetAdminMenuKeyboard());
            return;
        }
    
        var result = await MediatR.Send(new GetPagninateUserQueryRequest() { PageNumber = userPagination.Page, PageSize = userPagination.PageSize });

        // Show Users
        if (!result.IsSuccess)
        {
            await RemovePagination(callback.Message.Chat.Id);
            await botClient.SendTextMessageAsync(
                chatId: callback.Message.Chat.Id,
                text: result.Message!.ToStringEnumerable() ?? "خطایی پیش آمده",
                replyMarkup: CreateKeyboardHelper.GetAdminMenuKeyboard());
            return;
        }
        // Update userPagination
        userPagination.IsLast = result.Data.IsLast;
        userPagination.Page++;
        // Show Users
    
        if (result.Data.IsLast)
        {
            await botClient.SendTextMessageAsync(
           chatId: callback.Message.Chat.Id,
           text: GetUserInfoASOneString(result.Data.users, result.Data.TotalRecord)
           );
            await RemovePagination(callback.Message.Chat.Id);
            await botClient.SendTextMessageAsync(
         chatId: callback.Message.Chat.Id,
         text: "شما به انتهای لیست رسیدید",
         replyMarkup: CreateKeyboardHelper.GetAdminMenuKeyboard());
            return;
          
        }
        await botClient.SendTextMessageAsync(
         chatId: callback.Message.Chat.Id,
         text: GetUserInfoASOneString(result.Data.users, result.Data.TotalRecord),
         replyMarkup: CreateKeyboardHelper.Get_Pagniate_Cancel_Menu_KeyBoards());
        return;

    }

    private async Task PaginationQuery(int pageSize, ITelegramBotClient botClient, CallbackQuery query, BotAction botAction, UserDto user, CancellationToken cancellationToken)
    {

        await AddPagination(chatId: query.Message.Chat.Id, pageSize: pageSize);
        var result = await MediatR.Send(new GetPagninateUserQueryRequest() { PageNumber = 1, PageSize = pageSize });
        if (!result.IsSuccess)
        {
            await RemovePagination(query.Message.Chat.Id);
            await botClient.SendTextMessageAsync(
                chatId: query.Message.Chat.Id,
                text: result.Message!.ToStringEnumerable() ?? "خطایی پیش آمده",
                replyMarkup: CreateKeyboardHelper.GetAdminMenuKeyboard());
            return;
        }
        // Show Users
        var userPaginationBot = GetUserPaginationBot(query.Message.Chat.Id);
        userPaginationBot.IsLast = result.Data.IsLast;
        userPaginationBot.Page = 2;

      
        if(result.Data.IsLast)
        {
            await RemovePagination(query.Message.Chat.Id);
            await botClient.SendTextMessageAsync(
           chatId: query.Message.Chat.Id,
           text: GetUserInfoASOneString(result.Data.users, result.Data.TotalRecord)
    );
            await botClient.SendTextMessageAsync(
         chatId: query.Message.Chat.Id,
         text: "شما به انتهای لیست رسیدید",
         replyMarkup: CreateKeyboardHelper.GetAdminMenuKeyboard());
        }
        await botClient.SendTextMessageAsync(
           chatId: query.Message.Chat.Id,
           text: GetUserInfoASOneString(result.Data.users, result.Data.TotalRecord),
           replyMarkup: CreateKeyboardHelper.Get_Pagniate_Cancel_Menu_KeyBoards());
        return;
    }
    public UserPaginationBot GetUserPaginationBot(long chatId)
    {
        PaginationHelper.UserPaginationHelepr.TryGetValue(chatId, out UserPaginationBot userPagination);
        return userPagination;            
    }
    public string GetUserInfoASOneString(List<UserDto> users,long totalUsers)
    {
        var sb= new StringBuilder();
        sb.AppendLine($"تعداد کل کاربران سایت برابر است با : {totalUsers}");
        foreach (var user in users)
        {
            var activeText = user.IsActive == true ? "فعال" : "مسدود";
     
           
            sb.AppendLine($"آیدی کاربر : {user.Id}");
            sb.AppendLine($"آیدی چت بات تلگرام : {user.BotChatId}");
            sb.AppendLine($"شماره همراه : {user.PhoneNumber}");
            sb.AppendLine($"وضعیت فعالی : {activeText}");
            sb.AppendLine(GetCarsNumber(user.Cards));
            sb.AppendLine();
            sb.AppendLine("-----------------------------");
            sb.AppendLine();
            
     
        }
        return sb.ToString();
        static string GetCarsNumber(IEnumerable<UserCardsDto>? userCards)
        {
            var sb = new StringBuilder();
            if (userCards != null)
            {
                foreach (var card in userCards)
                {
                    sb.AppendLine($"شماره کارت : {card.CardNumber} ---- آیدی : {card.Id}");
                }
            }
            var txt = sb.ToString();
            return string.IsNullOrWhiteSpace(txt) == true ? "شماره کارتی ثبت نشده" : txt;
        }
    }
    private async Task RemovePagination(long id)
    {
        PaginationHelper.UserPaginationHelepr.Remove(id);
        await RemoveFromActionHelper(id);
    }

    private async Task AddPagination(long chatId , int pageSize )
    {
        PaginationHelper.UserPaginationHelepr.Remove(chatId);
        PaginationHelper.UserPaginationHelepr.Add(chatId, new UserPaginationBot
        {
            PageSize = pageSize,
            Page = 1,
            IsLast = false,
        });
    }
    private async Task BotOnMessageReceived(Message message,UserDto user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);
       
     
        if (BotSettings.StopSelling && user.Roles.Id != RoleName.Admin)
        {
            await _botClient.SendTextMessageAsync(chatId: user.BotChatId, "خرید در حال حاظر متوقف شده است");
            await RemoveAllPermantActions(user.BotChatId);
            await _botClient.DeleteMessageAsync(user.BotChatId, messageId: patientMessage.MessageId, cancellationToken);

            return;
        }
        if (message.Text is not { } messageText)
            return;
        _logger.LogInformation("Receive message type: {Messagetext}", message.Text);
        if (!user.IsActive)
        {
            await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "دسترسی شما این بات گرفته شده و اجازه دسترسی ندارید");
            return;
        }
        ActionHelper.BotActions.TryGetValue(message.Chat.Id, out BotAction botAction);
        // Handle BotActions
        if(botAction != null)
        {
            _logger.LogInformation($"User Bot Id {user.BotChatId} has action {botAction.ActionName}");
            if (message.Text == BotNameHelper.CancelAction || message.Text == BotNameHelper.AdminMenu || message.Text == BotNameHelper.Menu ||
                message.Text == BotNameHelper.SeeMenu || message.Text == BotNameHelper.BackToMenu)
            {
                await RemoveAllPermantActions(message.Chat.Id);
            }
            else
            {
                await HandleActions(_botClient, message, botAction, user, cancellationToken);
                return;
            }
        }

        _logger.LogInformation($"User Bot Id {user.BotChatId} has message {messageText}");

        if (user.Roles.Id == RoleName.Admin)
        {

            var action = messageText switch
            {

                BotNameHelper.Law => ShowLaws(_botClient, message, cancellationToken),
                BotNameHelper.AboutUs => AboutUs(_botClient, message, cancellationToken),
                BotNameHelper.BuyingProduct => BuyingProduct(_botClient, message, cancellationToken),
                BotNameHelper.BuyVoicher => ShowMenuBuyingVoicher(_botClient, user, message, cancellationToken),
                BotNameHelper.Cards => Cards(_botClient, message, cancellationToken),
                BotNameHelper.AddNewCard => NewCard(_botClient, message, cancellationToken),
                BotNameHelper.RegisteredCards => RegisteredCards(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel => AdminPanel(_botClient, message, cancellationToken),
                BotNameHelper.AdminMenu => AdminMenu(_botClient, message, cancellationToken),
                BotNameHelper.PurchasedVuchers => ShowPurchasedVuchers(_botClient,user, message, cancellationToken),
                // 
                // AdminPanel Key

                BotNameHelper.AdminPanel_BanUser => AdminPanel_BanUser(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel_UnBanUser => AdminPanel_UnBanUser(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel_SetLaws => AdminPanel_SetLaws(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel_SendMessageToAllUser => AdminPanel_SendMessageToAllUser(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel_StartSell => AdminPanel_StartSell(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel_StopSell => AdminPanel_StopSell(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel_StopBot => AdminPanel_StopBot(_botClient, message, cancellationToken),
                BotNameHelper.AdminPanel_SeeUsers => AdminPanel_SeeUsers(_botClient,user, message, cancellationToken),
                BotNameHelper.Menu=> AdminMenu(_botClient, message, cancellationToken),
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
                BotNameHelper.BuyVoicher => ShowMenuBuyingVoicher(_botClient,user, message, cancellationToken),
                BotNameHelper.PurchasedVuchers => ShowPurchasedVuchers(_botClient, user, message, cancellationToken),
                BotNameHelper.Cards => Cards(_botClient, message, cancellationToken),
                BotNameHelper.AddNewCard => NewCard(_botClient, message, cancellationToken),
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
                text: "منو",
                replyMarkup: CreateKeyboardHelper.GetUserMenuKeyBoard(),
                cancellationToken: cancellationToken);
        }
    }

    private async Task<Message> ShowPurchasedVuchers(ITelegramBotClient botClient, UserDto user, Message message, CancellationToken cancellationToken)
    {
       var result = await MediatR.Send(new GetVoicherCodeByBotChatIdRequest(user.BotChatId));
        if (!result.IsSuccess)
        {
            var msg = result.Message?.ToStringEnumerable();
            return await botClient.SendTextMessageAsync
                (chatId: user.BotChatId,
                text: msg ?? "خطا"
                , replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
        }
        foreach (var res in result.Data)
        {
            var msg = CreateString($"شماره سفارش :{res.OrderId}", $"بات آیدی کاربر :{res.UserBotChatId}", $"کد ووچر :{res.VoicherCode}");
             await botClient.SendTextMessageAsync
              (chatId: user.BotChatId,
              text: msg
              );
        }
        return await botClient.SendTextMessageAsync
         (chatId: user.BotChatId,
         text: "اتمام لیست"
         , replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey());
    }

    private async Task<Message> ShowMenuBuyingVoicher(ITelegramBotClient botClient,UserDto user, Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("----enter ShowMenuBuyingVoicher----");
        AddActionToActionHelper(user.BotChatId, new BotAction { ActionName = BotNameHelper.BuyVoicher });

        var result = await MediatR.Send(new GetUserCardsRequest(user.BotChatId) { });
        if(!result.IsSuccess)
        {
            await RemoveAllPermantActions(user.BotChatId);
            return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: "مشکلی پیش آمده بعدا امتحان نمایید",
               replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
               cancellationToken: cancellationToken);
        }
        // دریافت قیمت ووچر
        var resultVoicherValue = await MediatR.Send(new GetOnTimeVoicherValueRequest());
        var voicherValue = resultVoicherValue.Data;

        var amoutVoicherTxt = CreateString("نرخ لحظه ای ووچر", $"قیمت هر ووچر به ریال : {voicherValue?.Rials}", $"قیمت هر ووچر به دلار : {voicherValue?.Dollars}");
        if(result.Data.UserCards.Count < 1)
        {
            var msg = CreateString("برای خرید ووچر ابتدا باید یک کارت بانکی ثبت کنید", "ابتدا کارت خود را ثبت نمایید","در صورت عدم ثبت کارت امکان خرید ندارید", amoutVoicherTxt);
            await RemoveAllPermantActions(user.BotChatId);
            return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: msg,
               replyMarkup: CreateKeyboardHelper.GetCardsMenuKeyBoard(),
               cancellationToken: cancellationToken);
        }

       var txt = CreateString(amoutVoicherTxt, "در صورتی قصد خرید دارید", "تعداد ووچری که قصد خرید آن را دارید وارد نمایید");
        
        return await botClient.SendTextMessageAsync(
           chatId: message.Chat.Id,
           text: txt,
           replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
           cancellationToken: cancellationToken);
       
    }

    private async Task<Message> AdminPanel_SeeUsers(ITelegramBotClient botClient,UserDto userDto, Message message, CancellationToken cancellationToken)
    {
        if(userDto.Roles.Id != RoleName.Admin)
        {
            return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: BotNameHelper.AccessDenied,
               replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
               cancellationToken: cancellationToken
                );
                
        }
        AddActionToActionHelper(chatId: message.Chat.Id, new BotAction() { ActionName = BotNameHelper.Pagnination });
        return await botClient.SendTextMessageAsync(
              chatId: message.Chat.Id,
              text: "تعداد کاربرانی که در هر بار قصد دیدن آن را دارید را انتخاب کنید 👇🏻",
              replyMarkup: CreateKeyboardHelper.GetUserPaginationMenuKeyBorad(),
              cancellationToken: cancellationToken
               );
    }
    private async Task<Message> AdminPanel_StopBot(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {

        if(BotSettings.StopBot)
        {
            return await botClient.SendTextMessageAsync(
         chatId: message.Chat.Id,
         text: "بات در حال حاظر هم متوقف شده",
         cancellationToken: cancellationToken
          );
        }
        AddActionToActionHelper(message.Chat.Id, new BotAction() { ActionName = BotNameHelper.AdminPanel_StopBot });
        string msg = CreateString("اگر قصد توقف ربات را دارید :", "تایید : توقف فعالیت ربات", "کنسل :کنسل کردن توقف ربات", "بازگشت به منو : کنسل کردن عملیات توقف و بر گشت به منو");


        return   await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: msg,
            replyMarkup: CreateKeyboardHelper.Get_Accept_UnAccept_Menu_MenuKeyBoard(),
            cancellationToken: cancellationToken
             );


    }
    private async Task<Message> AdminPanel_StartBot(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {

        if (!BotSettings.StopBot)
        {
            return await botClient.SendTextMessageAsync(
         chatId: message.Chat.Id,
         text: "بات در حال کار است",
         cancellationToken: cancellationToken
          );
        }
        AddActionToActionHelper(message.Chat.Id, new BotAction() { ActionName = BotNameHelper.AdminPanel_StartBot });
        string msg = CreateString("اگر قصد توقف ربات را دارید :", "تایید : توقف فعالیت ربات", "کنسل :کنسل کردن توقف ربات", "بازگشت به منو : کنسل کردن عملیات توقف و بر گشت به منو");


        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: msg,
            replyMarkup: CreateKeyboardHelper.Get_Accept_UnAccept_Menu_MenuKeyBoard(),
            cancellationToken: cancellationToken
             );


    }
    private async Task<Message> AdminPanel_StopSell(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        if (BotSettings.StopSelling)
        {
            return await botClient.SendTextMessageAsync(
         chatId: message.Chat.Id,
         text: "در حال حاظر فروش متوقف شده است",
         cancellationToken: cancellationToken
          );
        }
        AddActionToActionHelper(message.Chat.Id, new BotAction() { ActionName = BotNameHelper.AdminPanel_StopSell });
        string msg = CreateString("اگر قصد توقف فروش را دارید :", "تایید : توقف فروش", "کنسل :کنسل کردن", "بازگشت به منو : کنسل کردن و بر گشت به منو");
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: msg,
            replyMarkup: CreateKeyboardHelper.Get_Accept_UnAccept_Menu_MenuKeyBoard(),
            cancellationToken: cancellationToken
             );

    }
    private async Task<Message> AdminPanel_StartSell(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {

        if (!BotSettings.StopSelling)
        {
            return await botClient.SendTextMessageAsync(
         chatId: message.Chat.Id,
         text: "در حال حاظر فروش در حال انجام است",
         cancellationToken: cancellationToken
          );
        }
        AddActionToActionHelper(message.Chat.Id, new BotAction() { ActionName = BotNameHelper.AdminPanel_StartSell });
        string msg = CreateString("اگر قصد آغاز دوباره فروش را دارید :", "تایید : آغاز فروش", "کنسل :کنسل کردن", "بازگشت به منو : کنسل کردن و بر گشت به منو");
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: msg,
            replyMarkup: CreateKeyboardHelper.Get_Accept_UnAccept_Menu_MenuKeyBoard(),
            cancellationToken: cancellationToken
             );

     

    }
    private async Task<Message> AdminPanel_SendMessageToAllUser(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {

        ActionHelper.BotActions.Add(message.Chat.Id, new BotAction { ActionName = BotNameHelper.SendToAllUser });
        return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: "پیغامی قصد ارسال آن برای همه کاربران را دارید را وارد نمایید.",
               replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
               cancellationToken: cancellationToken);
    }
    private async Task<Message> AdminPanel_SetLaws(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        
        AddActionToActionHelper(message.Chat.Id,new BotAction { ActionName = BotNameHelper.AdminPanel_SetLaws});
        
        return await botClient.SendTextMessageAsync(
          chatId: message.Chat.Id,
          text: "قوانین خود را وارد نمایید ، دقت کنید که قانون های قبلی پاک خواهند شد",
          replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
          cancellationToken: cancellationToken);
    }
    private string CreateString(params string[] texts) {
        StringBuilder sb = new StringBuilder();
        foreach (var text in texts)
        {
            sb.AppendLine(text);
        }
        return sb.ToString();
    }
    private async Task<Message> AdminPanel_UnBanUser(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        AddActionToActionHelper(message.Chat.Id, new BotAction { ActionName = BotNameHelper.AdminPanel_UnBanUser });

        return await botClient.SendTextMessageAsync(
          chatId: message.Chat.Id,
          text: "آیدی کاربری که قصد unban کردن آن را دارید را وارید نمایید.",
          replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
          cancellationToken: cancellationToken);
    }
    private async Task<Message> AdminPanel_BanUser(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        AddActionToActionHelper(message.Chat.Id, new BotAction { ActionName = BotNameHelper.AdminPanel_BanUser});

        return await botClient.SendTextMessageAsync(
          chatId: message.Chat.Id,
          text: "آیدی کاربری که قصد بن کردن آن را دارید را وارد نماید.",
          replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
          cancellationToken: cancellationToken);
    }
    private async Task<Message> AdminPanel(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        return await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "پنل ادمین",
            replyMarkup: CreateKeyboardHelper.GetAdminPanelKeyBoard(),
            cancellationToken: cancellationToken);
    }
    private async Task HandleActions(ITelegramBotClient botClient, Message message,BotAction botAction,UserDto userDto, CancellationToken cancellationToken)
    {
     

        switch (botAction.ActionName)
        {
            case BotNameHelper.SeeRegisteredCards :
            {
                    await SeeRegisteredCards(botClient, botAction, message, cancellationToken);
                    return;
            }
            case BotNameHelper.AddNewCard:
                {
                    await CheckNumberNewCard(botClient, botAction, message, cancellationToken);
                    return;
                }
            case BotNameHelper.BuyVoicher:
                {
                    await VerifyBuyVoicher(botClient,userDto, botAction, message, cancellationToken);
                    return;
                }
            default: break;
        }

        if(userDto.Roles.Id == RoleName.Admin)
        {
            switch (botAction.ActionName)
            {
                case BotNameHelper.SendToAllUser:
                    {
                        await SendMessageToAllUser(botClient, botAction, message, cancellationToken);
                        return;
                    }
                case BotNameHelper.AdminPanel_SetLaws:
                    {
                        await SetLaws(botClient, botAction, message, cancellationToken);
                        return;
                    }
                case BotNameHelper.AdminPanel_BanUser:
                    {
                        await BanUser(botClient, botAction, message, cancellationToken);
                        return;
                    }
                case BotNameHelper.AdminPanel_UnBanUser:
                    {
                        await UnBanUser(botClient, botAction, message, cancellationToken);
                        return;
                    }
                default: break;
            }
        }

    }

    private async Task VerifyBuyVoicher(ITelegramBotClient botClient, UserDto userDto, BotAction botAction, Message message, CancellationToken cancellationToken)
    {
        var convertNumberToEngNumber = NumberConverter.ConvertToEnglishNumbers(message.Text);
        ValidateCountBuyVoicher validations = new ValidateCountBuyVoicher();
        var resultValidate = await validations.ValidateAsync(new ValidateCountBuyVoicherTxt(message.Text));

        if (resultValidate.Errors.Count > 0)
        {
            await botClient.SendTextMessageAsync(
              chatId: message.Chat.Id,
              text: resultValidate.Errors.ToStringErrors(),
              replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
              cancellationToken: cancellationToken);
            return;
        }
        var resultVoicherValue = await MediatR.Send(new GetOnTimeVoicherValueRequest());
        if (resultVoicherValue == null || resultVoicherValue.IsSuccess == false)
        {
            await RemoveAllPermantActions(userDto.BotChatId);
            await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: resultVoicherValue?.Message?.ToStringEnumerable() ?? "خطایی پیش آمده بعدا امتحان نمایید",
            replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
            cancellationToken: cancellationToken);
            return;
        }
        var amoutVoicherTxt = CreateString(" باتوجه نرخ لحظه ای ووچر"
            , $"قیمت هر ووچر به ریال : {resultVoicherValue.Data?.Rials}",
            $"قیمت هر ووچر به دلار : {resultVoicherValue.Data?.Dollars}");
         
        var count = Convert.ToInt32(convertNumberToEngNumber);
        botAction.Count = count;
        var txt = CreateString($"تعدادی که شما وارد کردید :{convertNumberToEngNumber}"
            , amoutVoicherTxt,$"قیمت ریالی برای شما :{count * resultVoicherValue.Data.Rials}",
            $"قیمت دلاری برای شما :{((long)count)*resultVoicherValue.Data.Dollars}",
            "در صورت تایید از منو زیر تایید را انتخاب کنید تا لینک پرداخت برای شما ارسال شود");
        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, txt, replyMarkup: CreateKeyboardHelper.GetAcceptCancelKeyBoardBoard());
    }

    private async Task UnBanUser(ITelegramBotClient botClient, BotAction botAction, Message message, CancellationToken cancellationToken)
    {
        // Validate UserCard 
        var botChatId = NumberConverter.ConvertToEnglishNumbers(message.Text?.Trim());
        // Code 
        BotChatIdValidator botIdValidator = new BotChatIdValidator();
        var validateResult = await botIdValidator.ValidateAsync(new BotChatIdDto(botChatId) {  });
        if (!validateResult.IsValid)
        {
           
             await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: validateResult.Errors.ToStringErrors(),
               replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
               cancellationToken: cancellationToken);
            return;
        }
 
        botAction.Message = message.Text.Trim();
        var txt = CreateString("آیدی زیر آیدی کاربریست توسط شما دریافت گردید", message.Text.Trim(), "در صورت تایید از منو زیر تایید را انتخاب کنید");
        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, txt, replyMarkup: CreateKeyboardHelper.GetAcceptCancelKeyBoardBoard());
    }

    private async Task BanUser(ITelegramBotClient botClient, BotAction botAction, Message message, CancellationToken cancellationToken)
    {
        // Validate UserCard 
        var botChatId = NumberConverter.ConvertToEnglishNumbers(message.Text?.Trim());
        // Code 
        BotChatIdValidator botIdValidator = new BotChatIdValidator();
        var validateResult = await botIdValidator.ValidateAsync(new BotChatIdDto(botChatId) { });
        if (!validateResult.IsValid)
        {

            await botClient.SendTextMessageAsync(
              chatId: message.Chat.Id,
              text: validateResult.Errors.ToStringErrors(),
              replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
              cancellationToken: cancellationToken);
            return;
        }

        botAction.Message = message.Text.Trim();
        var txt = CreateString("آیدی زیر آیدی کاربریست توسط شما دریافت گردید", message.Text.Trim(), "در صورت تایید از منو زیر تایید را انتخاب کنید");
        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, txt, replyMarkup: CreateKeyboardHelper.GetAcceptCancelKeyBoardBoard());
    }

    private async Task SetLaws(ITelegramBotClient botClient, BotAction botAction, Message message, CancellationToken cancellationToken)
    {
       
        if (string.IsNullOrWhiteSpace(message.Text))
        {

            await botClient.SendTextMessageAsync(
              chatId: message.Chat.Id,
              text: "پیام وارد شده از سوی شما فرمت نا معتبر دارد دوباره تلاش فرمایید.",
              replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
              cancellationToken: cancellationToken);
            return;
        }

        botAction.Message = message.Text.Trim();
        var txt =CreateString("متن زیر توسط شما دریافت گردید", botAction.Message, "در صورت تایید از منو زیر تایید را انتخاب کنید");
        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, txt, replyMarkup: CreateKeyboardHelper.GetAcceptCancelKeyBoardBoard());
    }

    private async Task SendMessageToAllUser(ITelegramBotClient botClient, BotAction botAction, Message message, CancellationToken cancellationToken)
    {
       
        if(string.IsNullOrWhiteSpace(message.Text))
        {
             await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "پیام فرمت صحیحی ندارد دوباره امتحان کنید",
            cancellationToken: cancellationToken);
            return;
        }
        if(message.Text != BotNameHelper.AcceptSendMessage )
        {
            botAction.Message = message.Text;
            var textMessage = CreateString("پیام از جهت ارسال همگانی از جانب شما دریافت گردیده",
                "در صورتی که آن را قبول دارید بر روی باتن بله",
                "و در صورتی که قصد لغو این کار را دارید بر روی دکمه کنسل کلیک کنید.",
                " پیام زیر از جهت شما دریافت گردید 👇🏻 ",message.Text
                );
            await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: textMessage,
            replyMarkup: CreateKeyboardHelper.GetAcceptCancelKeyBoardBoard(),
            cancellationToken: cancellationToken);
            return;
        }

        var sendMessage = botAction.Message;
        await RemoveFromActionHelper(message.Chat.Id);
        var resultUsers = await MediatR.Send(new GetAllActiveUserChatIdRequest());
        if(!resultUsers.IsSuccess)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "خطایی پیش آمده");
        }
        await botClient.SendMessageToAll(users: resultUsers.Data, sendMessage, cancellationToken);
        return;
    }
    private async Task<Message> SeeRegisteredCards(ITelegramBotClient botClient,BotAction botAction, Message message, CancellationToken cancellationToken)
    {
        if(botAction.ActionName != BotNameHelper.SeeRegisteredCards) { return message; }
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
        await RemoveFromActionHelper(message.Chat.Id);
        return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: text,
               replyMarkup: new ReplyKeyboardRemove(),
               cancellationToken: cancellationToken);

    }
    private async Task<Message> RegisteredCards(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {


        var resultCards = await MediatR.Send(new GetUserCardsRequest(message.Chat.Id));
        if (!resultCards.IsSuccess)
        {
            return await botClient.SendTextMessageAsync(
          chatId: message.Chat.Id,
          text: "خطایی پیش آمده بعدا امتحان کنید",
          replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
          cancellationToken: cancellationToken);
        }
        else
        {
            string txtMessage;
            if(resultCards.Data.UserCards == null || resultCards.Data.UserCards.Count == 0)
            {
                txtMessage = "کارتی برای شما ثبت نشده";
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var userCard in resultCards.Data.UserCards)
                {
                    sb.AppendLine($"شمارت ثبت شده : {userCard.CardNumber}");
                }
                txtMessage = sb.ToString(); 
            }
            return await botClient.SendTextMessageAsync(
       chatId: message.Chat.Id,
       text: txtMessage,
       replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
       cancellationToken: cancellationToken);
        }
           
    
    }
    private async Task RemoveFromActionHelper(long chatId)
    {
        ActionHelper.BotActions.Remove(chatId);
    }
    private void AddActionToActionHelper(long chatId,BotAction botAction)
    {
        ActionHelper.BotActions.TryGetValue(chatId, out BotAction perBotAction); 
        if(perBotAction!= null) { ActionHelper.BotActions.Remove(chatId); }
        ActionHelper.BotActions.Add(chatId, botAction);
    }
    private async Task<Message> CheckNumberNewCard(ITelegramBotClient botClient,BotAction botAction, Message message, CancellationToken cancellationToken)
    {
        if (botAction.ActionName != BotNameHelper.AddNewCard ) return message;

        // Validate UserCard 
        var cardNumber = NumberConverter.ConvertToEnglishNumbers(message.Text?.Trim());
        // Code 
        CardNumberValidator cardNumberValidator = new CardNumberValidator();
        var validateResult = await cardNumberValidator.ValidateAsync(new CardNumberPresentaionDto() { CardNumber = cardNumber });
        if (!validateResult.IsValid)
        {
           
            return await botClient.SendTextMessageAsync(
               chatId: message.Chat.Id,
               text: validateResult.Errors.ToStringErrors(),
               replyMarkup: CreateKeyboardHelper.GetMenuCancelKeyBoard(),
               cancellationToken: cancellationToken);
        }
        var msg = CreateString("شماره کارت زیر از جهت شما دریافت گردیده", "اگر شما صاحب این کارت هستید",
            "و  قصد اضافه کردن این کارت را دارید",
            "میتوانید از منو نشان داده شده با تایید آن را به کارت ها خود اضافه کنید",
            $"شماره کارت دریافت شده :{cardNumber}", "تایید:اضافه شدن کارت", "کنسل:لغو عملیات"
            );
        botAction.Message = cardNumber;
        return await botClient.SendTextMessageAsync(
              chatId: message.Chat.Id,
              text: msg,
              replyMarkup: CreateKeyboardHelper.GetAcceptCancelKeyBoardBoard(),
              cancellationToken: cancellationToken);

   
    }
    private async Task<Message> NewCard(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {

        AddActionToActionHelper(message.Chat.Id, new BotAction { ActionName = BotNameHelper.AddNewCard, ActionStatus = ActionStatus.OnProccess });

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
            text: "Admin Menu",
            replyMarkup: CreateKeyboardHelper.GetAdminMenuKeyboard(),
            cancellationToken: cancellationToken);
    }
    private async Task<Message> Cards(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        return await botClient.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 text: "منو کارت ها",
                 replyMarkup: CreateKeyboardHelper.GetCardsMenuKeyBoard(),
                 cancellationToken: cancellationToken);
    }
    private async Task<Message> BuyingProduct(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        return await botClient.SendTextMessageAsync(
          chatId: message.Chat.Id,
          text:"به بخش خرید خوش آمدید",
          replyMarkup: CreateKeyboardHelper.GetBuyProductKeyBoard(),
          cancellationToken: cancellationToken);
    }
    private async Task<Message> AboutUs(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var settings = await MediatR.Send(new GetBotSettingsRequest(), cancellationToken);
        return await botClient.SendTextMessageAsync(
           chatId: message.Chat.Id,
           text: settings.AboutUs,
           replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
           cancellationToken: cancellationToken);
    }
    private  async Task<Message> ShowLaws(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var settings = await MediatR.Send(new GetBotSettingsRequest(), cancellationToken);
        return await botClient.SendTextMessageAsync(
           chatId: message.Chat.Id,
           text: settings.RuleTextAsOneString,
           replyMarkup: CreateKeyboardHelper.GetMenuKeyBoardsKey(),
           cancellationToken: cancellationToken);
    }
    private async Task<UserDto> CreateUser(Message message,CancellationToken cancellationToken)
    {
        var number = message.Contact?.PhoneNumber;
        
        if (number == null)
        {
            var ShareContactKeyboard = CreateKeyboardHelper.GetContactKeyboard();
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
        UserDto? newUser = new UserDto() { BotChatId = message.Chat.Id,PhoneNumber =  number,Roles = roleDto ,IsActive = true };

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
    private Task UnknownUpdateHandlerAsync(Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore IDE0060 // Remove unused parameter
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable RCS1163 // Unused parameter.
}
