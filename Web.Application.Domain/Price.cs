using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain.Base;

namespace Web.Application.Domain
{
    public class Price : BaseDateEntity
    {
        public Katana Katana { get; set; }
        public string KatanaCode { get; set; } = Guid.NewGuid().ToString();
        public decimal Amount { get; set; }
    }
}
