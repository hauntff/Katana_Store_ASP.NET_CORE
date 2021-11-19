using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain.Base;

namespace Web.Application.Domain
{
    public class Manufacturer : BaseEntity
    {
        public string Country { get; set; }
    }
}
