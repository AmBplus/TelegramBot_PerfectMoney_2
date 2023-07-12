using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Extensions
{
    public static class StringExtensionHelper
    {
        public static string ToStringEnumerable(this IEnumerable<string> enumTexts)
        {
            var sb = new StringBuilder();
            if (enumTexts != null)
                foreach (var enumText in enumTexts)
                {
                    sb.Append(enumText);
                }
            else return null;
            return sb.ToString();
        }
        public static string CreateString(params string[] strings)
        {
            StringBuilder sb = new StringBuilder(); 
            foreach (var s in strings)
            {
                sb.AppendLine(s);   
            }
            return sb.ToString();   
        }


        public static string CreateString(IEnumerable<string> strings)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in strings)
            {
                sb.AppendLine(s);
            }
            return sb.ToString();
        }
    }
}
