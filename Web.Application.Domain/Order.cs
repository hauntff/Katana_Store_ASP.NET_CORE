using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain.Base;
using Web.Application.Domain.Enums;

namespace Web.Application.Domain
{
    public class Order : BaseDateEntity
    {
        [Display(Name = "Пользователь")]
        public string UserName { get; set; }
        [Display(Name = "Статус")]
        public OrderStatusEnum Status { get; set; }
        [Display(Name = "К оплате")]
        public decimal Total { get; set; }
    }
}
