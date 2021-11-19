using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Data;
using Web.Application.Infrastructure.Interfaces;

namespace Web.Application.Infrastructure.Repo
{
    public class ManufacturerRepository : GenericRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(KatanaDbContext context) : base(context) { }

        public async Task<Manufacturer> GetByCountry(string country)
        {
            var newContext = (KatanaDbContext)context;
            var result = await newContext.Manufacturers
                .Where(p => p.Country == country).ToListAsync();

            var result0 = from p in newContext.Manufacturers
                          where p.Country == country
                          select p;

            return result0.FirstOrDefault();
        }
    }
}
