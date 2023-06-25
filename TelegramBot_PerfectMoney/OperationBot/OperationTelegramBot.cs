using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot_PerfectMoney.DataBase;
using TelegramBot_PerfectMoney.Helper;
using TelegramBot_PerfectMoney.Model;
using static System.Net.Mime.MediaTypeNames;

namespace TelegramBot_PerfectMoney.OperationBot
{
    public class OperationTelegramBot:IOperationTelegramBot
    {
        // private static int number = 1;
        private static string SavePhonNumber { get; set; }
        private TelContext _context { get; set; }
        private PersianCalendar Persian { get; set; }
        public OperationTelegramBot(TelContext context)
        {
            _context = context;
            Persian = new PersianCalendar();
            
        }

        public  async Task Start(ITelegramBotClient botClient,Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles)
                .FirstOrDefaultAsync();
            if (role == null)
            {
                var ShareContactKeyboard = CreatKeyboard.GetContactKeyboard();
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "لطفا شماره تلفن خود را ارسال کنید",
                    replyMarkup: ShareContactKeyboard,
                    cancellationToken: cancellationToken);
                return;
            }
           else if (role.Roles.Role == "Admin")
            {
                if (role.ChatId == null)
                {
                    role.ChatId = update.Message.Chat.Id.ToString();
                    _context.Update(role);
                    _context.SaveChanges();
                }
                var mainKeyboardMarkup = CreatKeyboard.SetMainKeyboardMarkupForAdmin();
                UserStepHandler.DeleteAll(update.Message.Chat.Id.ToString());
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "به بات خرید پرفکت مانی خوش آمدید.",
                    replyMarkup: mainKeyboardMarkup,
                    cancellationToken: cancellationToken);
                return;
            }
            else
            {
                var mainKeyboardMarkup = CreatKeyboard.SetMainKeyboardMarkupForUser();
                UserStepHandler.DeleteAll(update.Message.Chat.Id.ToString());
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "به بات خرید پرفکت مانی خوش آمدید.",
                    replyMarkup: mainKeyboardMarkup,
                    cancellationToken: cancellationToken);
            }
            // var user = new userModel()
            // {
            //     ChatId = update.Message!.Chat.Id,
            //     CodeId = "1111111",
            //     FirstName = "ali",
            //     LastName = "hey",
            //     MessageId = update.Message.MessageId,
            //     PhoneNumber = "09104",
            //     UserNameTelegram = update.Message.Chat.Username!
            // };
            // _context.Users.Add(user);
            // _context.SaveChanges();
            
            
        }

        public async Task AdminMainSection(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles)
                .FirstOrDefaultAsync();
            if (role is not  null&&role?.Roles?.Role == "Admin")
            {
                var CheckSelling = _context.botSettings.Select(x => x.StopSelling).FirstOrDefault();
                if (CheckSelling == false)
                {
                    var Adminkeyboard = CreatKeyboard.SetAdminStopSellingKeyboard();
                    UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), Adminkeyboard);
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: update.Message!.Chat.Id,
                        text: "پنل مدیریت",
                        replyMarkup: Adminkeyboard,
                        cancellationToken: cancellationToken);
                    return;
                }
                else
                {
                    var Adminkeyboard = CreatKeyboard.SetAdminActiveSellingMainKeyboard();
                    UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), Adminkeyboard);
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: update.Message!.Chat.Id,
                        text: "پنل مدیریت",
                        replyMarkup: Adminkeyboard,
                        cancellationToken: cancellationToken);
                    return;
                }
            }
            else
            {
               await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
           
        }

        public async Task GetContact(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var ShareContactKeyboard = CreatKeyboard.GetContactKeyboard();
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: update.Message!.Chat.Id,
                text: "لطفا شماره تلفن خودروا ارسال کنید",
                replyMarkup: ShareContactKeyboard,
                cancellationToken: cancellationToken);
        }

        public async Task AdminUserListSection(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var AdminUser = CreatKeyboard.UserListKeyboard();
                UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), AdminUser);
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "بخش مدیریت کاربران",
                    replyMarkup: AdminUser,
                    cancellationToken: cancellationToken);
                return;
            }
            else
            {
               await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
        }

        public async Task BackToMainSection(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                UserStepHandler.DeleteAll(update.Message.Chat.Id.ToString());
                var mainKeyboardMarkup = CreatKeyboard.SetMainKeyboardMarkupForAdmin();

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "صفحه اصلی",
                    replyMarkup: mainKeyboardMarkup,
                    cancellationToken: cancellationToken);
                return;
            }
            else
            {
                UserStepHandler.DeleteAll(update.Message.Chat.Id.ToString());
                var mainKeyboardMarkup = CreatKeyboard.SetMainKeyboardMarkupForUser();

                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "صفحه اصلی",
                    replyMarkup: mainKeyboardMarkup,
                    cancellationToken: cancellationToken);
                return;
            }
        }

        public async Task BackToPreviousnStep(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;

            UserStepHandler.RemoveCurentUserLastStep(chatId.ToString());

            var lastStep = UserStepHandler.GetUserLastStep(chatId.ToString());
            
            if (lastStep != null)
            {
                await botClient.SendTextMessageAsync(chatId, $"یکی از گزینه های زیر را انتخاب کنید",replyMarkup:lastStep, cancellationToken: cancellationToken);
                
            }
            else
            {
                // در صورت عدم وجود مرحله قبلی
               await botClient.SendTextMessageAsync(chatId, "مرحله قبلی یافت نشد لطفا وارد صفحه اصلی شوید.", cancellationToken: cancellationToken);
            }
        }

        public async Task GetUserList(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken,string page)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var key = CreatKeyboard.PaginitionUserListKeyboard();
                var AllCount = _context.Users.Count();

                var result = await _context.Users.Where(x=>x.RoleId !=1).Include(x=>x.Roles).Skip((Convert.ToInt32(page) - 1) * 10).Take(10).ToListAsync();
                if (result.Count == 0)
                {
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "اطلاعات وجود ندارد", cancellationToken: cancellationToken);
                    return;
                }

                var text = new StringBuilder();
                ;
                foreach (var item in result)
                {
                    var activeText = item.Active == true ? "فعال" : "مسدود";
                    var year = Persian.GetYear(item.CreationDate).ToString();
                    var month = Persian.GetMonth(item.CreationDate).ToString();
                    var day = Persian.GetDayOfMonth(item.CreationDate).ToString();
                    var hour = Persian.GetHour(item.CreationDate);
                    var minute = Persian.GetMinute(item.CreationDate);
                    text.AppendLine($"{result.IndexOf(item) + 1}- نام : {item.FirstName} ------ نام خانوادگی : {item.LastName}");
                    text.AppendLine($"نام کاربری : {item.UserNameTelegram} ------ شماره همراه : {item.PhoneNumber}");
                    text.AppendLine($"کد ملی : {item.CodeId} ------ وضعیت فعالی : {activeText}");
                    text.AppendLine($"تاریخ ثبت نام : {year}/{month}/{day} ------ ساعت ثبت نام : {hour}:{minute}");
                    text.AppendLine();
                    text.AppendLine("------------------------------------------------");
                    text.AppendLine();
                }

                if (result.Count < 10 && AllCount < 10)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, text.ToString(), cancellationToken: cancellationToken,replyMarkup:CreatKeyboard.BackKeyboards());
                    UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), CreatKeyboard.BackKeyboards());

                    return;
                }
                if (result.Count < 10 )
                {
                    await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, text.ToString(), cancellationToken: cancellationToken);
                    await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId, new InlineKeyboardMarkup(new InlineKeyboardButton("لیست قبل") { CallbackData = "لیست قبل" }), cancellationToken: cancellationToken);
                    return;
                }


                if (update.CallbackQuery is not null)
                {
                    if (Convert.ToInt32(page) == 1)
                    {
                        await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, text.ToString(), cancellationToken: cancellationToken);
                        await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                            update.CallbackQuery.Message.MessageId, new InlineKeyboardMarkup(new InlineKeyboardButton("لیست بعد") { CallbackData = "لیست بعد" }), cancellationToken: cancellationToken);
                        return;
                    }
                    await botClient.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId, text.ToString(), cancellationToken: cancellationToken);
                    await botClient.EditMessageReplyMarkupAsync(update.CallbackQuery.Message.Chat.Id,
                        update.CallbackQuery.Message.MessageId, key, cancellationToken: cancellationToken);
                }
                else
                {
                    var count = _context.Users.Count();
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                        $"مجموع کاربران : {count}", replyMarkup: CreatKeyboard.BackKeyboards(), cancellationToken: cancellationToken);
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, text.ToString(), replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton("لیست بعد") { CallbackData = "لیست بعد" }),
                        cancellationToken: cancellationToken);
                    UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), CreatKeyboard.BackKeyboards());
                }
                return;
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
            
           

        }

        public async Task SendNumberRequest(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // var stringBuilder = new StringBuilder();
            // stringBuilder.AppendLine("لطفا شماره کاربر را با فرمت زیر وارد کنید.");
            // stringBuilder.AppendLine("شماره همراه : ****09");
            // await botClient.SendTextMessageAsync(update.Message.Chat.Id,stringBuilder.ToString(),
            //     cancellationToken: cancellationToken,replyMarkup:CreatKeyboard.BackKeyboards());
            // UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(),CreatKeyboard.BackKeyboards());
        }

        public async Task SearchUserByPhoneNumber(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var extractNumber = ConvertHelper.ExtractNumberFromText(update.Message.Text);

                var checkPhone = ConvertHelper.IsIranianPhoneNumber(extractNumber);
                if (!checkPhone)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                         "فرمت شماره تلفن اشتباه هست لطفا دوباره ارسال کنید.", cancellationToken: cancellationToken);
                    return;
                }


                var result = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == extractNumber);
                if (result is null)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                        "این کاربر در سیستم موجود نمیباشد.میتوانید شماره دیگری را ارسال کنید.", cancellationToken: cancellationToken);
                    return;
                }
                var activeText = result.Active == true ? "فعال" : "مسدود";
                var year = Persian.GetYear(result.CreationDate).ToString();
                var month = Persian.GetMonth(result.CreationDate).ToString();
                var day = Persian.GetDayOfMonth(result.CreationDate).ToString();
                var hour = Persian.GetHour(result.CreationDate);
                var minute = Persian.GetMinute(result.CreationDate);
                var text = new StringBuilder();
                text.AppendLine($"نام : {result.FirstName} ------ نام خانوادگی : {result.LastName}");
                text.AppendLine($"نام کاربری : {result.UserNameTelegram} ------ شماره همراه : {result.PhoneNumber}");
                text.AppendLine($"کد ملی : {result.CodeId} ------ وضعیت فعالی : {activeText}");
                text.AppendLine($"تاریخ ثبت نام : {year}/{month}/{day} ------ ساعت ثبت نام : {hour}:{minute}");
                text.AppendLine($"موجودی:---- مجموع سفارشات:-----");
                text.AppendLine();
                SavePhonNumber = result.PhoneNumber;
                if (result.Active)
                {
                    var button = CreatKeyboard.BlockUser();
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, text.ToString(),
                        cancellationToken: cancellationToken, replyMarkup: button);
                    UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), button);
                    return;
                }
                else
                {
                    var button = CreatKeyboard.ActivUser();
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, text.ToString(),
                        cancellationToken: cancellationToken, replyMarkup: button);
                    UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), button);
                    return;
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
           
        }

        public async Task ActiveUser(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == SavePhonNumber);
                if (user == null)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "این کاربر وجود ندارد",
                        cancellationToken: cancellationToken);

                    return;
                }

                user.Active = true;
                _context.SaveChanges();
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "کاربر با موفقیت فعال شد",
                    cancellationToken: cancellationToken, replyMarkup: CreatKeyboard.BlockUser());
                UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), CreatKeyboard.BlockUser());
                var chatid = new ChatId(user.ChatId);
                
               
                await botClient.SendTextMessageAsync(chatid, "تبریک! کاربر محترم مسدودیت شما برطرف شد.",
                    cancellationToken: cancellationToken);
                return;
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }

        }

        public async Task BlockUser(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == SavePhonNumber);
                if (user == null)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "این کاربر وجود ندارد",
                        cancellationToken: cancellationToken);
                    return;
                }

                user.Active = false;
                _context.SaveChanges();
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "کاربر مسدود شد",
                    cancellationToken: cancellationToken, replyMarkup: CreatKeyboard.ActivUser());
                UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), CreatKeyboard.ActivUser());
                var chatid = new ChatId(user.ChatId);

                
                await botClient.SendTextMessageAsync(Convert.ToInt64(user.ChatId),
                    "کاربر محترم شما مسدود شدید. لطفا به ادمین پیام دهید",cancellationToken:cancellationToken);
              
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
        }

        public async Task SendMessageToUser(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.PhoneNumber == SavePhonNumber);
                if (user == null)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "این کاربر وجود ندارد",
                        cancellationToken: cancellationToken);
                    return;
                }

                var convert = new ChatId(user.ChatId);

                await botClient.SendTextMessageAsync(convert.Identifier, update.Message.Text,
                    cancellationToken: cancellationToken);
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "پیام با موفقیت ارسال شد.",
                    replyMarkup: CreatKeyboard.BackKeyboards(), cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
                
        }

        public async Task SendMessageToAllUsers(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var users = await _context.Users.Select(x => x.ChatId).ToListAsync();
                foreach (var item in users)
                {
                    if (item != update.Message.Chat.Id.ToString())
                    {
                        await botClient.SendTextMessageAsync(item, update.Message.Text, cancellationToken: cancellationToken);
                    }
                }

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "پیام با موفقیت ارسال شد");
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
        }

        public async Task ActivSelling(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var stopSelling = await _context.botSettings.FirstOrDefaultAsync();
                stopSelling.StopSelling = false;
                _context.SaveChanges();
                var Adminkeyboard = CreatKeyboard.SetAdminStopSellingKeyboard();
                UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), Adminkeyboard);
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "فروش شروع شد",
                    replyMarkup: Adminkeyboard,
                    cancellationToken: cancellationToken);
                var AllUser = await _context.Users.Where(x => x.RoleId != 1).Include(x => x.Roles).Select(x=>x.ChatId).ToListAsync();
                foreach (var item in AllUser)
                {
                    await botClient.SendTextMessageAsync(Convert.ToInt64(item),
                        "پیام به تمامی اعضای محترم: فروش شروع شد.", cancellationToken: cancellationToken);
                }

            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
        }

        public async Task StopSelling(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var stopSelling = await _context.botSettings.FirstOrDefaultAsync();
                stopSelling.StopSelling = true;
                _context.SaveChanges();
                var Adminkeyboard = CreatKeyboard.SetAdminActiveSellingMainKeyboard();
                UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), Adminkeyboard);
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "فروش متوقف شد",
                    replyMarkup: Adminkeyboard,
                    cancellationToken: cancellationToken);
                var AllUser = await _context.Users.Where(x => x.RoleId != 1).Include(x => x.Roles).Select(x => x.ChatId).ToListAsync();
                foreach (var item in AllUser)
                {
                    await botClient.SendTextMessageAsync(Convert.ToInt64(item),
                        "پیام به تمامی اعضای محترم: فروش متوقف شد.", cancellationToken: cancellationToken);
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }

        }

        public async Task StopBot(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles).Select(x=>x.Roles.Role)
                .FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var chatId = await _context.Users.Where(x => x.ChatId != update.Message.Chat.Id.ToString()).Select(x => x.ChatId).ToListAsync();
                foreach (var item in chatId)
                {
                    await botClient.SendTextMessageAsync(item, "ربات در دست تعمیر", cancellationToken: cancellationToken);
                }
                await botClient.DeleteWebhookAsync(false, cancellationToken: cancellationToken);
                await botClient.CloseAsync(cancellationToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "دسترسی شما به این قسمت مجاز نیس.",
                    cancellationToken: cancellationToken);
            }
        }

        public async Task SaveContact(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var user = await _context.Users.Where(x => x.PhoneNumber == update.Message.Contact.PhoneNumber)
                .Include(x => x.Roles).FirstOrDefaultAsync();
            if (user == null)
            {
                var NewUser = new userModel()
                {
                    ChatId = update.Message.Chat.Id.ToString(),
                    CodeId = null,
                    FirstName = update.Message.Contact.FirstName,
                    LastName = update.Message.Contact.LastName,
                    PhoneNumber = update.Message.Contact.PhoneNumber,
                    UserNameTelegram = update.Message.Chat.Username,
                    RoleId = 2

                };
                _context.Users.Add(NewUser);
                _context.SaveChanges();
               await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                    "شماره تلفن شما ثبت شد لطفا برای تکمیل اطلاعات به پنل احراز هویت مراحعه فرمایید.",
                    cancellationToken: cancellationToken,replyMarkup:CreatKeyboard.SetMainKeyboardMarkupForUser());
            }

           else if (user.Roles.Role == "Admin")
            {
                user.FirstName = user.FirstName ?? update.Message.Contact.FirstName;
                user.LastName = user.LastName ?? update.Message.Contact.LastName;
                user.ChatId = user.ChatId ?? update.Message.Chat.Id.ToString();
                _context.Update(user);
                _context.SaveChanges();
               await botClient.SendTextMessageAsync(update.Message.Chat.Id, "خوش آمدید",
                    cancellationToken: cancellationToken,replyMarkup:CreatKeyboard.SetMainKeyboardMarkupForAdmin());

               // UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(),CreatKeyboard.SetMainKeyboardMarkupForAdmin());
            }
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, $"به بات پرفکت مانی خوش آمدید.",
                    cancellationToken: cancellationToken, replyMarkup: CreatKeyboard.SetMainKeyboardMarkupForAdmin());
                // UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(), CreatKeyboard.SetMainKeyboardMarkupForAdmin());
            }
        }

        public async Task SendRuleTextToAdmin(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles)
                .Select(x => x.Roles.Role).FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var RuleText = await _context.botSettings.Select(x => x.RuleText).FirstOrDefaultAsync();
                if (RuleText == null)
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                        "متن قوانین وجود ندارد لطفا متنی وارد کنید.", replyMarkup: CreatKeyboard.BackKeyboards(),
                        cancellationToken: cancellationToken);
                    UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(),CreatKeyboard.BackKeyboards());
                }
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("لطفا برای تغییر متن قوانین بات، متن خود را با فرمت زیر بنویسید. جهت سهولت میتوانید از این متن کپی بگیرید.");
                stringBuilder.AppendLine("متن قوانین: ");
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, RuleText,
                    cancellationToken: cancellationToken, replyMarkup: CreatKeyboard.BackKeyboards());
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, stringBuilder.ToString(),
                    cancellationToken: cancellationToken);
                UserStepHandler.AddUserStep(update.Message.Chat.Id.ToString(),CreatKeyboard.BackKeyboards());
                return;
            }

            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "کاربر محترم شما به این قسمت دسترسی ندارید.",
                cancellationToken: cancellationToken);
            return;

        }

        public async Task SendRule(ITelegramBotClient botClient, Update update, CancellationToken cancellation)
        {
            var role = await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles)
                .Select(x => x.Roles.Role).FirstOrDefaultAsync();
            if (role == "Admin")
            {
                var textRule = await _context.botSettings.FirstOrDefaultAsync();
                var seprator = "متن قوانین:";
                int index = update.Message.Text.IndexOf(seprator) + seprator.Length;
                var result =update.Message.Text.Substring(index);
                textRule.RuleText = result;
                _context.Update(textRule);
                _context.SaveChanges();
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "متن قوانین با موفقیت تغییر کرد",
                    cancellationToken: cancellation);
                return;
            }
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "کاربر محترم شما به این قسمت دسترسی ندارید.",
                cancellationToken: cancellation);
            return;
        }

        public async Task GetRuleText(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var user =await _context.Users.Where(x => x.ChatId == update.Message.Chat.Id.ToString()).Include(x => x.Roles)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                    "لطفا بات را غیرفعال کرده و سپس /start را وارد نمایید", cancellationToken: cancellationToken);
                return;
            }

            var textRule = await _context.botSettings.Select(x => x.RuleText).FirstOrDefaultAsync();
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, textRule,
                cancellationToken: cancellationToken);
            UserStepHandler.DeleteAll(update.Message.Chat.Id.ToString());
        }
    }
}
