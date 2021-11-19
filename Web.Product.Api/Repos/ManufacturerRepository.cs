using Microsoft.EntityFrameworkCore;
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
    public class ManufacturerRepository : GenericRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(ProductDbContext context) : base(context) { }

        public async Task<Manufacturer> GetByCountry(string title)
        {
            var newContext = (ProductDbContext)context;
            var result = await newContext.Manufacturers
                .Where(p => p.Country == title).ToListAsync();

            var result0 = from p in newContext.Manufacturers
                          where p.Country == title
                          select p;

            return result0.FirstOrDefault();
        }
    }
}
