using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain;

namespace Web.Order.Api.Interfaces
{
    public interface IStoreService
    {
        //ask<Katana> GetKatanaById(int katanaId);
        Task<Web.Application.Domain.Order> CreateOrUpdateOrder(int katanaId, string title, int priceId, decimal price, string userName);
        Task<bool> AddToBusket(Web.Application.Domain.Order order, int katanaId, string title, int priceId, decimal price);
        bool DeleteFromBusket(Web.Application.Domain.Order order, int katanaId);
        bool CompleteOrder(int orderId);
        Task<Web.Application.Domain.Order> FindOrder(int orderId);
        //IEnumerable<Katana> FindKatanas(string title);
        Task<Web.Application.Domain.Order> GetLastUnpayedOrder(string userName);
        //Task<IEnumerable<Katana>> GetKatanas();
        //Task<IEnumerable<Manufacturer>> GetManufacturers();
    }
}
