using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Settings
{
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable RCS1110 // Declare type inside namespace.
    public class PerfectMonneySettings
    {
#pragma warning restore RCS1110 // Declare type inside namespace.
#pragma warning restore CA1050 // Declare types in namespaces
        public static readonly string Configuration = "PerfectMonneySettings";
        public string AccountID { get; set; }
        public string PassPhrase { get; set; }
        public string PayerAccount { get; set; }
    }
}
