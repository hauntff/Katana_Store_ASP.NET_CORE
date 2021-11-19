using Store.Books.Domain;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;

namespace Web.Application.Infrastructure.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment> { }
}
