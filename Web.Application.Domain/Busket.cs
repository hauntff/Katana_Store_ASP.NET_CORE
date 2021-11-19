using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain.Base;

namespace Web.Application.Domain
{
    public class Busket : BaseEntity
    {
        public string Code { get; set; } = Guid.NewGuid().ToString();
        public Order Order { get; set; }
        public int KatanaId { get; set; }
        public int PriceId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}
