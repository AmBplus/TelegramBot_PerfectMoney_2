using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot_PerfectMoney.OperationBot
{
    internal static class CreatKeyboard
    {
        private static ReplyKeyboardMarkup? mainKeyboardMarkupForAdmin { get; set; }
        private static ReplyKeyboardMarkup? AdminActiveSellingMainMarkup { get; set; }
        private static ReplyKeyboardMarkup? ShareContactKeyboradMarkup { get; set; }
        private static ReplyKeyboardMarkup? UserListKeyboardMarkup { get; set; }
        private static InlineKeyboardMarkup? PaginitionListMarkup { get; set; }
        private static ReplyKeyboardMarkup? BackKeyboardsMarkup { get; set; }
        private static ReplyKeyboardMarkup? ActiveKeyboardMarkup { get; set; }
        private static ReplyKeyboardMarkup? BlockKeyboardMarkup { get; set; }
        private static ReplyKeyboardMarkup? AdminStopSellingMainMarkup { get; set; }
        private static ReplyKeyboardMarkup? MainKeyboardMarkUpForUser { get; set; }
        public static ReplyKeyboardMarkup SetMainKeyboardMarkupForUser()
        {
            if (MainKeyboardMarkUpForUser is null)
            {
                MainKeyboardMarkUpForUser = new ReplyKeyboardMarkup(new []
                {
                    new KeyboardButton[] { "خرید 💸", "موجودی 💳"},
                    new KeyboardButton[] { "احراز هویت 🔒", "قوانین ⚖️" },
                }){ResizeKeyboard = true};
            }

            return MainKeyboardMarkUpForUser;
        }
        public static ReplyKeyboardMarkup BackKeyboards()
        {
            if (BackKeyboardsMarkup is null)
            {
                BackKeyboardsMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "بازگشت به مرحله قبل" },
                        new KeyboardButton[] { "صفحه اصلی" },
                    })
                    { ResizeKeyboard = true };
            }
            return BackKeyboardsMarkup;
        }

        public static ReplyKeyboardMarkup SetAdminStopSellingKeyboard()
        {
            if (AdminStopSellingMainMarkup is null)
            {
                AdminStopSellingMainMarkup = new(new[]
                    {
                        new KeyboardButton[]{ "لیست کاربران 📄", "ارسال پیام همگانی 📧" },
                        new KeyboardButton[]{ "توقف فروش 🛑", "در دست تعمیر 🛠️" },
                        new KeyboardButton[]{"صفحه اصلی", "تنظیم قوانین ⚖" }

                    })
                    { ResizeKeyboard = true };
            }

            return AdminStopSellingMainMarkup;
        }
        public static InlineKeyboardMarkup PaginitionUserListKeyboard()
        {
            
            if (PaginitionListMarkup is null)
            {
                PaginitionListMarkup = new InlineKeyboardMarkup(new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "لیست قبل", callbackData: "لیست قبل"),
                    InlineKeyboardButton.WithCallbackData(text: "لیست بعد", callbackData: "لیست بعد"),
                });
            }

            return PaginitionListMarkup;
        }
        public static ReplyKeyboardMarkup UserListKeyboard()
        {
            if (UserListKeyboardMarkup is  null)
            {
                UserListKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] { "نمایش کاربران \U0001f9d1", "جستجو 🔎" },
                    // new KeyboardButton[] { "", "فعال کردن کاربر ✔️" },
                    new KeyboardButton[] { "بازگشت به مرحله قبل"  },
                    new KeyboardButton[] { "صفحه اصلی"  },

                }) { ResizeKeyboard = true };
            }

            return UserListKeyboardMarkup;
        }
        public static ReplyKeyboardMarkup SetMainKeyboardMarkupForAdmin()
        {
            if (mainKeyboardMarkupForAdmin == null)
            {
                mainKeyboardMarkupForAdmin = new(new[]
                {
                    new KeyboardButton[] { "خرید 💸", "موجودی 💳"},
                    new KeyboardButton[] { "احراز هویت 🔒", "قوانین ⚖️" },
                    new KeyboardButton[] { "مدیریت " + "👨🏼‍💼" },
                })
                {
                    ResizeKeyboard = true
                };
            }
            return mainKeyboardMarkupForAdmin;

        }

        public static ReplyKeyboardMarkup ActivUser()
        {
            if (ActiveKeyboardMarkup is null)
            {
                ActiveKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "فعال کردن کاربر ✔️" },
                        new KeyboardButton[] { "ارسال پیام به کاربر 📧", "لیست سفارشات کاربر 📄" },
                        new KeyboardButton[] { "مدیریت " + "👨🏼‍💼", "صفحه اصلی" }

                    })
                    { ResizeKeyboard = true };
            }
            return ActiveKeyboardMarkup;
        }

        public static ReplyKeyboardMarkup BlockUser()
        {
            if (BlockKeyboardMarkup == null)
            {
               BlockKeyboardMarkup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "مسدود کردن کاربر 🚧" },
                        new KeyboardButton[] { "ارسال پیام به کاربر 📧", "لیست سفارشات کاربر 📄" },
                        new KeyboardButton[] { "مدیریت "+ "👨🏼‍💼", "صفحه اصلی" }
                    })
                    { ResizeKeyboard = true };
            }

            return BlockKeyboardMarkup;
        }
        public static ReplyKeyboardMarkup SetAdminActiveSellingMainKeyboard()
        {
            if (AdminActiveSellingMainMarkup is null)
            {
                AdminActiveSellingMainMarkup = new(new[]
                {
                    new KeyboardButton[]{ "لیست کاربران 📄", "ارسال پیام همگانی 📧" },
                    new KeyboardButton[]{ "شروع فروش ✔️", "در دست تعمیر 🛠️" },
                    new KeyboardButton[]{"صفحه اصلی", "تنظیم قوانین ⚖" }

                }) { ResizeKeyboard = true };
            }

            return AdminActiveSellingMainMarkup;
        }

        public static ReplyKeyboardMarkup GetContactKeyboard()
        {
            if (ShareContactKeyboradMarkup is null)
            {
                ShareContactKeyboradMarkup = new(new[]
                {
                    KeyboardButton.WithRequestContact("ارسال شماره تلفن ☎️")
                }) { ResizeKeyboard = true };
            }

            return ShareContactKeyboradMarkup;
        }
    }
    
}
