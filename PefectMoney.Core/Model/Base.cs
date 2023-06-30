using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Model
{
    public abstract class Base<T>
    {
        public T Id { get; set; }
        public DateTime CreationDate{ get; set; }
    }
}
