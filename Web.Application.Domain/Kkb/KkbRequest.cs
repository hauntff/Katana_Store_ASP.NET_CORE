using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Web.Application.Domain.Kkb
{
    public class KkbRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Sign { get; set; }
    }
}
