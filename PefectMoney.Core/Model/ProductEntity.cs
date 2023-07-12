using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.Model
{
    public class ProductEntity : Base<long>
    {
        public ProductEntity() : base()
        {

        }
        public string Name { get; set; }
        
        public ICollection<OrderEntity> Orders { get; set; }

    }
    public enum ProductName : int
    {
        VoicherCode = 1 
    }
}
