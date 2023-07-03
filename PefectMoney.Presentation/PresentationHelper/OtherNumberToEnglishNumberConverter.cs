using System.Globalization;

namespace PefectMoney.Presentation.PresentationHelper
{
 

    public  class NumberConverter
    {
        public static string ConvertToEnglishNumbers(string input)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US"); // تنظیمات برای اعداد انگلیسی

            // تبدیل اعداد فارسی و عربی به اعداد انگلیسی
            string result = string.Empty;
            foreach (char ch in input)
            {
                if (char.IsDigit(ch))
                {
                    int number = int.Parse(ch.ToString());
                    result += number.ToString(cultureInfo);
                }
                else
                {
                    result += ch;
                }
            }

            return result;
        }
    }

}
