using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot_PerfectMoney.DataBase;
using TelegramBot_PerfectMoney.Helper;
using TelegramBot_PerfectMoney.Model;
using TelegramBot_PerfectMoney.OperationBot;

namespace TelegramBot_PerfectMoney.TelegramPresentation
{
    public class TelegramBot
    {
        private UserStepHandler stepHandler { get; set; }
        private int PageNumber { get; set; }
        private IOperationTelegramBot _operation { get; set; }
        private CancellationTokenSource cts { get; }
        private TelContext _context { get; set; }
        public IServiceProvider ServiceProvider { get; }

        public TelegramBot(IServiceProvider serviceProvider)
        {

            _operation = serviceProvider.GetService<IOperationTelegramBot>();
            _context = serviceProvider.GetService<TelContext>(); 
            cts = new();
            stepHandler = new();
            ServiceProvider = serviceProvider;
        }

        public async Task Run(string TokenBot)
        {


            var botClient = new TelegramBotClient(TokenBot);

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
            var me = await botClient.GetMeAsync();
            Console.Write("Start listening for");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" @{me.Username}");
            Console.ResetColor();
            // var users = await _context.Users.Where(x => x.RoleId != 1).Include(x => x.Roles).ToListAsync();
            // foreach (var item in users)
            // {
            //     var convert = Convert.ToInt64(item.ChatId);
            //    await botClient.SendTextMessageAsync(convert, "بات فعال شد");
            // }


            // Send cancellation request to stop bot
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // var ActiveUserCheck = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString())
            //     .Include(x => x.Roles).Select(x => x.Active).FirstOrDefaultAsync();
            // if (!ActiveUserCheck)
            // {
            //     await botClient.SendTextMessageAsync(update.Message.Chat.Id,
            //         "کاربر محترم شما مسدود شدید. لطفا به ادمین پیام دهید", cancellationToken: cancellationToken);
            //     return;
            // }
         
            if (update?.Message is not null || update.CallbackQuery is not null)
            {
                if (update.Message?.Text == "/start")
                {
                    // جهت چک کردن اینکه شماره تلفن کاربر ثبت شده یا نه
                    // if (Identity)
                    // {
                    //      
                    // }
                    await _operation.Start(botClient, update, cancellationToken);
                    // UserStepHandler.DeleteAll(update.Message.Chat.Id.ToString());
                    return;

                }
                else  if (update.Type == UpdateType.Message && update.Message.Contact is not null)
                {
                    await _operation.SaveContact(botClient, update, cancellationToken);
                    return;
                }
                else if(update.Type == UpdateType.CallbackQuery)
                {
                     if (update.CallbackQuery?.Data == "لیست بعد")
                    {
                        PageNumber += 1;
                        var result = UserStepHandler.GetUserLastStep(update.CallbackQuery.Message.Chat.Id.ToString());
                        await _operation.GetUserList(botClient, update, cancellationToken, PageNumber.ToString());
                    }
                    else if (update.CallbackQuery?.Data == "لیست قبل")
                    {
                        PageNumber -= 1;
                        if (PageNumber < 1)
                            PageNumber = 1;

                        await _operation.GetUserList(botClient, update, cancellationToken, PageNumber.ToString());
                        return;
                    }
                     return;
                }
                

               else if (update.Type == UpdateType.Message)
                {
                    var typkeyborad = UserStepHandler.GetUserLastStep(update.Message.Chat.Id.ToString());
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "درحال پردازش .....",
                        cancellationToken: cancellationToken);
                    #region About Admin Panel
                    

                    if (update.Message?.Text == "لیست کاربران 📄" )
                    {
                      
                            await _operation.AdminUserListSection(botClient, update, cancellationToken);
                            return;
                        
                        return;
                    }
                    else if(update.Message.Text == "قوانین ⚖️")
                    {
                        await _operation.GetRuleText(botClient, update, cancellationToken);
                        return;
                    }
                    else if (update.Message.Text == "تنظیم قوانین ⚖")
                    {
                      
                            await _operation.SendRuleTextToAdmin(botClient, update, cancellationToken);
                            return;
                        
                        return;
                    }
                    else if(update.Message.Text.Contains("متن قوانین") || update.Message.Text.Contains("قوانین"))
                    {
                        if (typkeyborad == CreatKeyboard.BackKeyboards())
                        {
                         await   _operation.SendRule(botClient, update, cancellationToken);
                         return;
                        }
                    }
                    else if (update.Message?.Text == "مدیریت " + "👨🏼‍💼" )
                    {
                        await _operation.AdminMainSection(botClient, update, cancellationToken);
                        return;

                    }
                    else if (update.Message?.Text == "نمایش کاربران 🧑" && typkeyborad == CreatKeyboard.UserListKeyboard())
                    {
                        if (PageNumber > 1 || PageNumber == 0)
                            PageNumber = 1;
                        await _operation.GetUserList(botClient, update, cancellationToken, PageNumber.ToString());
                        return;
                    }
                    else if (update.Message.Text == "ارسال پیام همگانی 📧" )
                    {
                        
                            var stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine("لطفا پیام خودرا با فرمت زیر وارد کنید. جهت سهولت میتوانید از این متن کپی بگیرید.");
                            stringBuilder.AppendLine("پیام به تمامی اعضای محترم :");
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, stringBuilder.ToString(),
                                cancellationToken: cancellationToken, replyMarkup: CreatKeyboard.BackKeyboards());
                            UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), CreatKeyboard.BackKeyboards());
                            return;
                        
                      return;
                    }
                    else if(update.Message.Text.Contains("پیام") || update.Message.Text.Contains("تمامی")||update.Message.Text.Contains("محترم"))
                    {
                        if (typkeyborad == CreatKeyboard.BackKeyboards())
                        {
                            await _operation.SendMessageToAllUsers(botClient, update, cancellationToken);
                            return;


                        }
                    }
                    else if (update.Message.Text == "شروع فروش ✔️")
                    {
                       
                           await _operation.ActivSelling(botClient, update, cancellationToken);
                            return;
                        
                       
                    }
                    else if (update.Message.Text == "توقف فروش \U0001f6d1")
                    {
                        
                           await _operation.StopSelling(botClient, update, cancellationToken);
                            return;
                        
                       
                    }
                    else if(update.Message.Text == "در دست تعمیر 🛠️")
                    {
                      
                            await _operation.StopBot(botClient, update, cancellationToken);
                            cts.Cancel();
                            return;
                        
                    
                    }
                    else if (update.Message?.Text == "جستجو 🔎" && typkeyborad == CreatKeyboard.UserListKeyboard())
                    {
                        var stringBuilder = new StringBuilder();
                        stringBuilder.AppendLine("لطفا شماره کاربر را با فرمت زیر وارد کنید. جهت سهولت میتوانید از این متن کپی بگیرید.");
                        stringBuilder.AppendLine("شماره همراه : +989");
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, stringBuilder.ToString(),
                            cancellationToken: cancellationToken, replyMarkup: CreatKeyboard.BackKeyboards());
                        UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), CreatKeyboard.BackKeyboards());
                        return;
                    }
                    else if(update.Message?.Text == "ارسال پیام به کاربر 📧" )
                    {
                        if (typkeyborad == CreatKeyboard.BlockUser() || typkeyborad == CreatKeyboard.ActivUser())
                        {
                            var stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine("لطفا پیام خودرا با فرمت زیر وارد کنید. جهت سهولت میتوانید از این متن کپی بگیرید.");
                            stringBuilder.AppendLine("پیام از طرف مدیر بات :");
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, stringBuilder.ToString(),
                                cancellationToken: cancellationToken, replyMarkup: CreatKeyboard.BackKeyboards());
                            UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), CreatKeyboard.BackKeyboards());
                            
                            return;
                        }
                        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, "عملیات نا معتبر",
                            cancellationToken: cancellationToken);
                        return;
                    }
                    else if (update.Message.Text.Contains("پیام از طرف مدیر بات") || update.Message.Text.Contains("مدیر"))
                    {
                        if (typkeyborad == CreatKeyboard.BackKeyboards())
                        {
                            await _operation.SendMessageToUser(botClient, update, cancellationToken);
                            return;
                        }
                       
                    }
                    else if (update.Message.Text.Contains("شماره همراه") || update.Message.Text.Contains("همراه")||update.Message.Text.Contains("شماره"))
                    {
                        if (typkeyborad == CreatKeyboard.BackKeyboards())
                        {
                            await _operation.SearchUserByPhoneNumber(botClient, update, cancellationToken);
                            return;
                        }
                        
                    }
                    else if(update.Message.Text == "مسدود کردن کاربر 🚧" )
                    {
                        if (typkeyborad == CreatKeyboard.BlockUser() || typkeyborad == CreatKeyboard.ActivUser())
                        {
                            await _operation.BlockUser(botClient, update, cancellationToken);
                            return;
                        }
                        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, "عملیات نا معتبر لطفا از ابتدا شروع کنید.",
                            cancellationToken: cancellationToken);
                        return;
                    }
                    else if(update.Message.Text == "فعال کردن کاربر ✔️")
                    {
                        if (typkeyborad == CreatKeyboard.BlockUser() || typkeyborad == CreatKeyboard.ActivUser())
                        {
                            await _operation.ActiveUser(botClient, update, cancellationToken);
                            return;
                        }
                        await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, "عملیات نا معتبر لطفا از ابتدا شروع کنید.",
                            cancellationToken: cancellationToken);
                        return;
                    }
                   
                    #endregion


                    else if (update.Message?.Text == "بازگشت به مرحله قبل")
                     {
                         await _operation.BackToPreviousnStep(botClient, update, cancellationToken);
                         return;
                     }
                     else if (update.Message?.Text == "صفحه اصلی")
                     {
                         await _operation.BackToMainSection(botClient, update, cancellationToken);
                         return;
                     }
                     else
                     {
                         await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, "عملیات نامعتبر. لطفا دوباره امتحان کنید.",
                             cancellationToken: cancellationToken);
                     }

                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id, "لطفا وارد صفحه اصلی شوید",
                        cancellationToken: cancellationToken);
                }
                



              
            }



        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {


            Console.Write("Error");
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $" : [{apiRequestException.ErrorCode}]  {apiRequestException.Message}",
                _ => " : " + exception.ToString()
            };
            Console.ResetColor();
            Console.WriteLine(ErrorMessage);
            Console.WriteLine("\n\n Press any key for close program......");
            Console.ReadKey();
            return Task.CompletedTask;
        }

        public void Stop()
        {
            cts.Cancel();
        }
    }
}
