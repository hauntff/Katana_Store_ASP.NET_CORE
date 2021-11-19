using Web.Application.Infrastructure.Interfaces;
using Web.Application.Infrastructure.Repo;
using Web.Order.Api.Data;

namespace Web.Order.Api.Repos
{
    public class OrderRepository : GenericRepository<Application.Domain.Order>, IOrderRepository
    {
        public OrderRepository(OrderDbContext context) : base(context) { }
    }
}
