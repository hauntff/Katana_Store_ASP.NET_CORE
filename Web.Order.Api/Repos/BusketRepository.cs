using Web.Application.Infrastructure.Interfaces;
using Web.Application.Infrastructure.Repo;
using Web.Order.Api.Data;

namespace Web.Order.Api.Repos
{
    public class BusketRepository : GenericRepository<Application.Domain.Busket>, IBusketRepository
    {
        public BusketRepository(OrderDbContext context) : base(context) { }
    }
}
