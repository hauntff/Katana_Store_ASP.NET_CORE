
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;
using Web.Application.Infrastructure.Repo;
using Web.Order.Api.Data;

namespace Web.Order.Api.Repos
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(OrderDbContext context) : base(context) { }
    }
}
