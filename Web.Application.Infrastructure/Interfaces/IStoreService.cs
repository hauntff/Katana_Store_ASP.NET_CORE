using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain;

namespace Web.Application.Infrastructure.Repo
{
    public interface IStoreService
    {
        Task<Katana> GetKatanaById(int katanaId);
        Task<Order> CreateOrUpdateOrder(int katanaId, string userName);
        Task<bool> AddToBusket(Order order, Katana katana, Price price);
        bool DeleteFromBusket(Order order, int katanaId);
        bool CompleteOrder(int orderId);
        Order FindOrder(int orderId);
        IEnumerable<Katana> FindKatanas(string title);
        //Order GetLastUnpayedOrder(string userName);
        Task<IEnumerable<Katana>> GetKatanas();
        Task<IEnumerable<Manufacturer>> GetManufacturers();
    }
}
