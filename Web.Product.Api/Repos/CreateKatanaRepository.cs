using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;
using Web.Application.Infrastructure.Repo;
using Web.Product.Api.CQRS.Commands;
using Web.Product.Api.Data;
using Web.Product.Api.Interfaces;

namespace Web.Product.Api.Repos
{
    public class CreateKatanaRepository : GenericRepository<CreateKatanaCommand>, ICreateKatanaRepository
    {
        public CreateKatanaRepository(ProductDbContext context) : base(context) { }
    }
}
