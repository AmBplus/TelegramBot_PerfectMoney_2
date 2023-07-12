using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace PefectMoney.Presentation.PresentationHelper.OperationBot
{
    internal static class CreateKeyboardHelper
    {

        private static ReplyKeyboardMarkup? ShareContactKeyboradMarkup { get; set; }
        private static ReplyKeyboardMarkup? CardsMenuKeyBoards{ get; set; }
        private static ReplyKeyboardMarkup? BuyProductMenuKeyBoards { get; set; }
        private static InlineKeyboardMarkup? Pagniate_Cancel_Menu_KeyBoards{ get; set; }
        private static InlineKeyboardMarkup? StopSelling_MenuKeyBoard{ get; set; }
        private static InlineKeyboardMarkup? Accept_UnAccept_Menu_MenuKeyBoard{ get; set; }
        private static InlineKeyboardMarkup? StopBot_MenuKeyBoard{ get; set; }
        private static InlineKeyboardMarkup? UserPaginationMenuKeyBorad{ get; set; }
        private static ReplyKeyboardMarkup? GetMenuKeyBoards{ get; set; }
        private static InlineKeyboardMarkup? AcceptCancelKeyBoard{ get; set; }
        private static ReplyKeyboardMarkup? MenuCancelKeyBoard{ get; set; }
        private static ReplyKeyboardMarkup? AdminLawKeyBoard{ get; set; }

        private static ReplyKeyboardMarkup? AdminMenuKeyboard { get; set; }
        private static ReplyKeyboardMarkup? AdminPanelKeyboard { get; set; }
        private static ReplyKeyboardMarkup? UserMenuKeyboard { get; set; }
        private static ReplyKeyboardMarkup? AdminActiveSellingMainMarkup { get; set; }

        private static ReplyKeyboardMarkup? UserListKeyboardMarkup { get; set; }

        private static ReplyKeyboardMarkup? BackKeyboardsMarkup { get; set; }
        private static ReplyKeyboardMarkup? ActiveKeyboardMarkup { get; set; }
        private static ReplyKeyboardMarkup? BlockKeyboardMarkup { get; set; }
        private static ReplyKeyboardMarkup? AdminStopSellingMainMarkup { get; set; }
        private static ReplyKeyboardMarkup? MainKeyboardMarkUpForUser { get; set; }
        public static InlineKeyboardMarkup Get_Accept_UnAccept_Menu_MenuKeyBoard()
        {
            if (Accept_UnAccept_Menu_MenuKeyBoard is null)
            {
                Accept_UnAccept_Menu_MenuKeyBoard = new(new[]
                {

                   new []
                 {
                        InlineKeyboardButton.WithCallbackData(text: "تایید", callbackData: BotNameHelper.AcceptAction),
                        InlineKeyboardButton.WithCallbackData(text: "کنسل", callbackData: BotNameHelper.CancelAction),
                },

                   new []
                {
                        InlineKeyboardButton.WithCallbackData(text: "منو اصلی", callbackData: BotNameHelper.BackToMenu),

                },

                });
            }


            return Accept_UnAccept_Menu_MenuKeyBoard;
        }
        public static InlineKeyboardMarkup Get_Pagniate_Cancel_Menu_KeyBoards()
        {
            if (Pagniate_Cancel_Menu_KeyBoards is null)
            {
                Pagniate_Cancel_Menu_KeyBoards = new(new[]
                {
  
                   new []
                 {
                        InlineKeyboardButton.WithCallbackData(text: "دیدن لیست بعدی کاربران", callbackData: BotNameHelper.Paginate_See_Next_Page),
                        InlineKeyboardButton.WithCallbackData(text: "لغو و بازگشت", callbackData: BotNameHelper.CancelAction),
                },
   
                   new []
                {
                        InlineKeyboardButton.WithCallbackData(text: "منو اصلی", callbackData: BotNameHelper.SeeMenu),
                     
                },
       
                });
            }


            return Pagniate_Cancel_Menu_KeyBoards;
        }
        public static InlineKeyboardMarkup GetUserPaginationMenuKeyBorad()
        {
            if (UserPaginationMenuKeyBorad is null)
            {
                UserPaginationMenuKeyBorad = new(new[]
                {
    // first row
                   new []
                 {
                        InlineKeyboardButton.WithCallbackData(text: "10", callbackData: BotNameHelper.Pagnination_Number10),
                        InlineKeyboardButton.WithCallbackData(text: "20", callbackData: BotNameHelper.Pagnination_Number20),
                },
      // second row
                   new []
                {
                        InlineKeyboardButton.WithCallbackData(text: "50", callbackData: BotNameHelper.Pagnination_Number50),
                        InlineKeyboardButton.WithCallbackData(text: "100", callbackData: BotNameHelper.Pagnination_Number100),
                },
                   new []
                {
                        InlineKeyboardButton.WithCallbackData(text: "200", callbackData: BotNameHelper.Pagnination_Number200),
                        InlineKeyboardButton.WithCallbackData(text: "400", callbackData: BotNameHelper.Pagnination_Number400),
                },
                   new []
               {
                        InlineKeyboardButton.WithCallbackData(text: "800", callbackData: BotNameHelper.Pagnination_Number800),
                        InlineKeyboardButton.WithCallbackData(text: "تمامی کاربران", callbackData: BotNameHelper.Pagination_AllNumber),
               },
                });
               }
           

            return UserPaginationMenuKeyBorad;
        }
        public static ReplyKeyboardMarkup GetMenuKeyBoardsKey()
        {
            if (GetMenuKeyBoards is null)
            {
                GetMenuKeyBoards = new(new[]
                    {
                        new KeyboardButton[]{ BotNameHelper.BackToMenu},
                    })
                { ResizeKeyboard = true };
            }

            return GetMenuKeyBoards;
        }
        public static InlineKeyboardMarkup GetAcceptCancelKeyBoardBoard()
        {
            if (AcceptCancelKeyBoard is null)
            {
                AcceptCancelKeyBoard = new(new[]
             {

                   new []
                 {
                        InlineKeyboardButton.WithCallbackData(text: "تایید", callbackData: BotNameHelper.AcceptAction),
                        InlineKeyboardButton.WithCallbackData(text: "کنسل", callbackData: BotNameHelper.CancelAction),
                },

                   new []
                {
                        InlineKeyboardButton.WithCallbackData(text: "منو اصلی", callbackData: BotNameHelper.BackToMenu),

                },

                });
            }
        

            return AcceptCancelKeyBoard;
        }
        public static ReplyKeyboardMarkup GetMenuCancelKeyBoard()
        {
            if (MenuCancelKeyBoard is null)
            {
                MenuCancelKeyBoard = new(new[]
                    {
                        new KeyboardButton[]{ BotNameHelper.CancelAction,BotNameHelper.SeeMenu},
                        new KeyboardButton[]{ BotNameHelper.BackToMenu},
                    })
                { ResizeKeyboard = true };
            }

            return MenuCancelKeyBoard;
        }
        public static ReplyKeyboardMarkup GetAdminLawKeyBoard()
        {
            if (AdminLawKeyBoard is null)
            {
                AdminLawKeyBoard = new(new[]
                    {
                        new KeyboardButton[]{ BotNameHelper.AdminPanel_SetLaws,BotNameHelper.AdminPanel_AddNewLaw},
                        new KeyboardButton[]{ BotNameHelper.BackToMenu},
                    })
                { ResizeKeyboard = true };
            }

            return AdminLawKeyBoard;
        }

        public static ReplyKeyboardMarkup GetBuyProductKeyBoard()
        {
            if (BuyProductMenuKeyBoards is null)
            {
                BuyProductMenuKeyBoards = new(new[]
                    {
                        new KeyboardButton[]{ BotNameHelper.BuyVoicher,BotNameHelper.BackToMenu},
                    })
                { ResizeKeyboard = true };
            }

            return BuyProductMenuKeyBoards;
        }


        public static ReplyKeyboardMarkup GetAdminPanelKeyBoard()
        {
            if (AdminPanelKeyboard is null)
            {
                AdminPanelKeyboard = new(new[]
                    {
                        new KeyboardButton[]{ BotNameHelper.AdminPanel_SeeUsers,BotNameHelper.AdminPanel_SendMessageToAllUser},
                        new KeyboardButton[]{ BotNameHelper.AdminPanel_StopBot,BotNameHelper.AdminPanel_SetLaws},
                        new KeyboardButton[]{ BotNameHelper.AdminPanel_StopSell,BotNameHelper.AdminPanel_StartSell},
                        new KeyboardButton[]{ BotNameHelper.AdminPanel_BanUser,BotNameHelper.AdminPanel_UnBanUser},
                        new KeyboardButton[]{ BotNameHelper.AdminMenu},
                    })
                { ResizeKeyboard = true };
            }

            return AdminPanelKeyboard;
        }

        public static ReplyKeyboardMarkup GetCardsMenuKeyBoard()
        {
            if (CardsMenuKeyBoards is null)
            {
                CardsMenuKeyBoards = new ReplyKeyboardMarkup(new[]
                {
                 new KeyboardButton[]{BotNameHelper.RegisteredCards ,BotNameHelper.AddNewCard},
                 new KeyboardButton[]{BotNameHelper.BackToMenu },
               
                })
                { InputFieldPlaceholder = "منو کارت ها", IsPersistent = true, ResizeKeyboard = true };
            }
            return CardsMenuKeyBoards;
        }
        public static ReplyKeyboardMarkup GetUserMenuKeyBoard()
        {
            if (UserMenuKeyboard is null)
            {
                UserMenuKeyboard = new ReplyKeyboardMarkup(new[]
                {
                 new KeyboardButton[]{BotNameHelper.Law ,BotNameHelper.Cards},
                 new KeyboardButton[]{BotNameHelper.BuyingProduct, BotNameHelper.PurchasedVuchers },
                 new KeyboardButton[]{BotNameHelper.AboutUs},
                })
                { InputFieldPlaceholder = "منو", IsPersistent = true, ResizeKeyboard = true };
            }
            return UserMenuKeyboard;
        }

        public static ReplyKeyboardMarkup SetMainKeyboardMarkupForUser()
        {
            if (MainKeyboardMarkUpForUser is null)
            {
                MainKeyboardMarkUpForUser = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "خرید 💸", "موجودی 💳"},
                    new KeyboardButton[] { "احراز هویت 🔒", "قوانین ⚖️" },
                })
                { ResizeKeyboard = true };
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

        public static ReplyKeyboardMarkup UserListKeyboard()
        {
            if (UserListKeyboardMarkup is null)
            {
                UserListKeyboardMarkup = new(new[]
                {
                    new KeyboardButton[] { "نمایش کاربران \U0001f9d1", "جستجو 🔎" },
                    // new KeyboardButton[] { "", "فعال کردن کاربر ✔️" },
                    new KeyboardButton[] { "بازگشت به مرحله قبل"  },
                    new KeyboardButton[] { "صفحه اصلی"  },

                })
                { ResizeKeyboard = true };
            }

            return UserListKeyboardMarkup;
        }
        public static ReplyKeyboardMarkup GetAdminMenuKeyboard()
        {

            if (AdminMenuKeyboard is null)
            {
                AdminMenuKeyboard = new ReplyKeyboardMarkup(new[]
                {
                 new KeyboardButton[]{BotNameHelper.Law ,BotNameHelper.Cards},
                 new KeyboardButton[]{BotNameHelper.BuyingProduct, BotNameHelper.PurchasedVuchers },
                 new KeyboardButton[]{BotNameHelper.AboutUs,BotNameHelper.AdminPanel},
                })
                { InputFieldPlaceholder = "منو", IsPersistent = true, ResizeKeyboard = true };
            }
            return AdminMenuKeyboard;

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

                })
                { ResizeKeyboard = true };
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
