using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;
using Web.Application.Infrastructure.Repo;
using Web.Product.Api.Data;

namespace Web.Product.Api.Repos
{
    public class KatanaManufacturerRepository : GenericRepository<KatanaManufacturer>, IKatanaManufacturerRepository
    {
        public KatanaManufacturerRepository(ProductDbContext context) : base(context) { }
    }
}
