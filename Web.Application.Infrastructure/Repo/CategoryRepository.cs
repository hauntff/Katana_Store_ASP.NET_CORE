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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(KatanaDbContext context) : base(context) { }

        public async Task<Category> GetByNameOfCategory(string nameOfCategory)
        {
            var newContext = (KatanaDbContext)context;
            var result = await newContext.Categories
                .Where(p => p.NameOfCategory == nameOfCategory).ToListAsync();

            var result0 = from p in newContext.Categories
                          where p.NameOfCategory == nameOfCategory
                          select p;

            return result0.FirstOrDefault();
        }
    }
}
