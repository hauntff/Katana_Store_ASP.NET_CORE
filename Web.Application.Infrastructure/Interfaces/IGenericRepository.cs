using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Web.Application.Infrastructure.Interfaces
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");

        Task<T> GetById(object id);

        Task Insert(T entity);

        Task Delete(object id);

        Task Delete(T entityToDelete);

        Task Update(T entityToUpdate);
    }
}
