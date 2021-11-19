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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ProductDbContext context) : base(context) { }
    }
}
