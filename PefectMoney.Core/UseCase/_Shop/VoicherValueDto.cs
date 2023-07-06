using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase._Shop
{
    public record VoicherValueDto
    {
        public double Dollars { get; set; }
        public int Rials { get; set; }
    }
}
