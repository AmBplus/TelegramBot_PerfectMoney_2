using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Model
{
    public class Product : Base<long>
    {

        public string Name { get; set; }
        public long OrderId { get; set; }
        public ICollection<Order> Orders { get; set; }

    }
}
