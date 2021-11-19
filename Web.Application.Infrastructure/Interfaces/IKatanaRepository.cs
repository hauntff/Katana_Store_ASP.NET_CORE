using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain;

namespace Web.Application.Infrastructure.Interfaces
{
    public interface IKatanaRepository : IGenericRepository<Katana>
    {
        Task<Katana> GetByTitle(string title);
        IEnumerable<Katana> GetPaged(int page, int perPage);
    }
}
