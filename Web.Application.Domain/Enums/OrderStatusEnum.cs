using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Domain.Enums
{
    public enum OrderStatusEnum
    {   
        Created = 0,
        Paying = 1,
        Payed = 2,
        Completed = 3,
        Cancelled = 4
    }
}
