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
            foreach (var enumText in enumTexts) { 
            sb.Append(enumText.ToString());}
            return sb.ToString();
        }
    }
}
