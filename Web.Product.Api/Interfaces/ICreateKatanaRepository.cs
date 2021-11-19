using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Infrastructure.Interfaces;
using Web.Product.Api.CQRS.Commands;

namespace Web.Product.Api.Interfaces
{
    public interface ICreateKatanaRepository : IGenericRepository<CreateKatanaCommand>
    {
    }
}
