using PefectMoney.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Model
{
    public abstract class Base<T>
    {
        public Base()
        {
            CreationDate = TimeHelper.DateTimeNow;
        }
        public T Id { get; set; }
        public DateTime CreationDate{ get; set; }
    }
}
