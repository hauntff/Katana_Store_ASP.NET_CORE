using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain;

namespace Web.Application.Infrastructure.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
    }
}
