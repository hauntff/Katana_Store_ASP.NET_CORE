using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Domain.Enums;
using Web.Application.Infrastructure.Data;
using Web.Application.Infrastructure.Interfaces;
using Web.Order.Api.Interfaces;

namespace Web.Order.Api.Services
{
    public class StoreService : IStoreService
    {
        private readonly IOrderRepository _orders;
        private readonly IPaymentRepository _payments;
        private readonly IBusketRepository _buskets;
        //private readonly KatanaDbContext _context;
        private readonly ILogger<StoreService> _logger;
        public StoreService(IOrderRepository orders,
            IPaymentRepository payments,
            IBusketRepository buskets,
            ILogger<StoreService> logger)
        {
            _orders = orders;
            _payments = payments;
            _buskets = buskets;
            _logger = logger;
        }
        public async Task<Application.Domain.Order> CreateOrUpdateOrder(int katanaId, string title, int priceId, decimal price, string userName)
        {
            if (0 >= katanaId)
            {
                _logger.LogWarning($"CreateOrUpdateOrder: katanaId <= 0: {katanaId}");
                throw new ArgumentOutOfRangeException($"katanaId <= 0: {katanaId}");
            }
            if (price < 0)
            {
                _logger.LogWarning($"CreateOrUpdateOrder: price amount too low for katanaId: {katanaId}");
                throw new ArgumentOutOfRangeException($"lastPrice not found for katanaId: {katanaId}");
            }
            var order = await GetLastUnpayedOrder(userName);
            if (order is null)
            {
                _logger.LogWarning($"CreateOrUpdateOrder: last order for user: {userName} not found, creating new.");
                await _orders.Insert(new Application.Domain.Order
                {
                    UserName = userName,
                    Status = OrderStatusEnum.Created,
                    Created = DateTime.Now,
                    Total = price
                });
                order = await GetLastUnpayedOrder(userName);
            }
            await AddToBusket(order, katanaId, title, priceId, price);
            return order;
        }
        public async Task<bool> AddToBusket(Application.Domain.Order order, int katanaId, string title, int priceId, decimal price)
        {
            var existing = await _buskets
                .Get(p => p.Order.Id == order.Id && p.KatanaId == katanaId, includeProperties: "Order");
            if (!(existing is null))
            {
                _logger.LogWarning($"AddToBusket: katanaId: {katanaId} exist in busket");
                return false;
            }
            await _buskets.Insert(new Busket { Order = order, KatanaId = katanaId, PriceId = priceId });
            return SafeSave();
        }
        public bool DeleteFromBusket(Application.Domain.Order order, int katanaId)
        {
            //var busket = ????;
            return false;
        }
        public async Task<Application.Domain.Order> FindOrder(int orderId)
        {
            return await _orders.GetById(orderId);
        }
        public bool CompleteOrder(int orderId)
        {
            throw new NotImplementedException();
        }
        public bool SafeSave()
        {
            try
            {
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
        public async Task<Application.Domain.Order> GetLastUnpayedOrder(string userName)
        {
            return (await _orders.Get(p => p.UserName == userName && p.Status == OrderStatusEnum.Created))
                .OrderByDescending(p => p.Created)
                .FirstOrDefault();
        }
    }
}
