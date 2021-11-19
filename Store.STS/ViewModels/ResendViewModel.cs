using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Store.STS.ViewModels
{
    public class ResendViewModel
    {
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string ResendToken { get; set; }
    }
}
