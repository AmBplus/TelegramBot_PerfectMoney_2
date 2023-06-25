using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot_PerfectMoney.Helper
{
    public  class UserStepHandler
    {
        private static Dictionary<string,List<ReplyKeyboardMarkup?>> userSteps { get; set; }
        private static int LastIndex { get; set; }
        public UserStepHandler()
        {
            userSteps = new();
            
        }

      public  static void AddUserStep(string chatId ,ReplyKeyboardMarkup KeyBoardMark)
        {

            // ایجاد لیست مراحل برای کاربر در صورت عدم وجود
            if (!userSteps.ContainsKey(chatId))
            {
                userSteps[chatId] = new List<ReplyKeyboardMarkup>();
            }

            if (userSteps[chatId].Count > 0  )
            {
                LastIndex = userSteps[chatId].LastIndexOf(userSteps[chatId].Last());
                if (LastIndex > 1)
                {
                    if (userSteps[chatId][LastIndex] == KeyBoardMark )
                    {
                        return;
                    }
                }
                
                
            }

            // var result = GetUserLastStep(chatId);
            // if (result == KeyBoardMark)
            //     return;
            // اضافه کردن مرحله به لیست
            userSteps[chatId].Add(KeyBoardMark);
        }

      // تابع برای حذف آخرین مرحله از کاربر
        public static void RemoveCurentUserLastStep(string chatId)
        {
            // بررسی وجود کاربر در دیکشنری
            if (userSteps.ContainsKey(chatId))
            {
                
                LastIndex = userSteps[chatId].LastIndexOf(userSteps[chatId].Last());

                // حذف آخرین مرحله از لیست
                userSteps[chatId].RemoveAt(LastIndex);
            }
        }

        // تابع برای دریافت آخرین مرحله کاربر
     public static ReplyKeyboardMarkup GetUserLastStep(string chatId)
        {
            // بررسی وجود کاربر در دیکشنری و وجود مرحله
            if (userSteps.ContainsKey(chatId) && userSteps[chatId].Count > 0)
            {
                // بازگرداندن آخرین مرحله
                LastIndex = userSteps[chatId].LastIndexOf(userSteps[chatId].Last());
                var result = userSteps[chatId][LastIndex];
                return result;
            }

            return null;
        }

     public static  void DeleteAll(string chatId)
        {
            if (userSteps.ContainsKey(chatId))
            {
                userSteps[chatId].Clear();
            }
            
        }
    }
}
