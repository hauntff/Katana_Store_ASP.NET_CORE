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
    public class KatanaRepository : GenericRepository<Katana>, IKatanaRepository
    {
        public KatanaRepository(KatanaDbContext context) : base(context) { }

        public async Task<Katana> GetByTitle(string title)
        {
            var newContext = (KatanaDbContext)context;
            var result = await newContext.Katanas
                .Where(p => p.Title == title).ToListAsync();

            var result0 = from p in newContext.Katanas
                          where p.Title == title
                          select p;

            return result0.FirstOrDefault();
        }
        public IEnumerable<Katana> GetPaged(int page, int perPage)
        {
            return base.GetPaged(page, perPage);
        }
    }
}
