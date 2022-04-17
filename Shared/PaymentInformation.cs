using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PaymentInformation
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
