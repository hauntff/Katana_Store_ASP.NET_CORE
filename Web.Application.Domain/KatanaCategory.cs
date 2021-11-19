using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain.Base;

namespace Web.Application.Domain
{
    public class KatanaCategory : BaseEntity
    {
        public Katana Katana { get; set; }
        public Category Category { get; set; }
    }
}
