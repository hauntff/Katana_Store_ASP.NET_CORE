using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain.Base;

namespace Web.Application.Domain
{
    public class Katana : BaseEntity
    {
        public string Code { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public decimal Lenght { get; set; }
        public string Color { get; set; }

    }

}
