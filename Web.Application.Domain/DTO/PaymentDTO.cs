using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Domain.DTO
{
    public class PaymentDTO
    {
        public string command { get; set; }
        public string account { get; set; }
        public string txn_id { get; set; }
        public string txn_date { get; set; }
        public decimal sum { get; set; }
    }
}
