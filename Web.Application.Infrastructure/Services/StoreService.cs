using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Data;
using Web.Application.Infrastructure.Repo;

namespace Web.Application.Infrastructure.Services
{
    public class StoreService : IStoreService
    {
        private readonly KatanaDbContext _context;
        private readonly ILogger _logger;
        public StoreService(KatanaDbContext context, ILogger<StoreService> logger)
        {
            _logger = logger;
            _context = context;
        }

        private async Task<Price> GetLastPrice(int katanaId)
        {
            _logger.LogInformation($"GetLastPrice get price by katanaId: {katanaId}");
            var lastPrice = await _context.Prices.Include(p => p.Katana).Where(p => p.Katana.Id == katanaId)
                .OrderByDescending(p => p.Created).FirstOrDefaultAsync();
            if (!(lastPrice is null))
                _logger.LogInformation($"GetLastPrice price value: {lastPrice.Amount} by katanaId: {katanaId}");
            else _logger.LogWarning($"GetLastPrice price value not found by katanaId: {katanaId}");
            return lastPrice;
        }
        private async Task<(Price, Exception)> GetLastPriceEx(int katanaId)
        {
            try
            {
                return (await GetLastPrice(katanaId), null);
            }
            catch (Exception exception)
            {
                _logger.LogError($"GetLastPriceEx error: {exception.Message}");
                return (null, exception);
            }
        }
        public async Task<Katana> GetKatanaById(int katanaId)
        {
            try
            {
                var katana = await _context.Katanas.FindAsync(katanaId);
                return katana;
            }
            catch (Exception exception)
            {
                _logger.LogError($"GetKatanaById error: {exception.Message}");
                return null;
            }
        }
        public async Task<Order> CreateOrUpdateOrder(int katanaId, string userName)
        {
            var katana = await GetKatanaById(katanaId);
            if (katana is null)
            {
                _logger.LogWarning($"CreateOrder: katana not found for katanaId: {katanaId}");
                throw new NullReferenceException($"katana not found for katanaId: {katanaId}");
            }
            var (lastPrice, _) = await GetLastPriceEx(katanaId);
            if (lastPrice is null)
            {
                _logger.LogWarning($"CreateOrder: lastPrice not found for katanaId: {katanaId}");
                throw new NullReferenceException($"lastPrice not found for katanaId: {katanaId}");
            }
            if (lastPrice.Amount < 0)
            {
                _logger.LogWarning($"CreateOrder: price amount too low for katanaId: {katanaId}");
                throw new ArgumentOutOfRangeException($"lastPrice not found for katanaId: {katanaId}");
            }
            var order = await _context
                .Orders
                .Where(p => p.UserName == userName && p.Status == Domain.Enums.OrderStatusEnum.Created)
                .OrderByDescending(p => p.Created)
                .FirstOrDefaultAsync();
            if (order is null)
            {
                _logger.LogWarning($"CreateOrder: last order for user: {userName} not found, creating new.");
                var orderEntry = _context.Orders.Add(new Order
                {
                    UserName = userName,
                    Status = Domain.Enums.OrderStatusEnum.Created,
                    Created = DateTime.Now,
                    Total = lastPrice.Amount
                });
                await SafeSave();
                order = orderEntry.Entity;
            }
            return order;
        }
        public async Task<bool> AddToBusket(Order order, Katana katana, Price price)
        {
            var existing = await _context.Buskets
                .Include("Order")
                .Include("Katana")
                .FirstOrDefaultAsync(p => p.Order.Id == order.Id && p.KatanaId == katana.Id);
            if (!(existing is null))
            {
                _logger.LogWarning($"AddToBusket: katanaId: {katana.Id} exist in busket");
                return false;
            }
            _context.Buskets.Add(new Busket { Order = order, KatanaId = katana.Id, PriceId = price.Id });
            return await SafeSave();
        }
        public bool DeleteFromBusket(Order order, int katanaId)
        {
            //var busket = ????;
            return false;
        }
        public Order FindOrder(int orderId)
        {
            return _context.Orders.FirstOrDefault(p => p.Id == orderId);
        }
        public bool CompleteOrder(int orderId)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> SafeSave()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }
        public async Task<IEnumerable<Katana>> GetKatanas()
        {
            return await _context.Katanas.ToListAsync();
        }
        public IEnumerable<Katana> FindKatanas(string title)
        {
            return _context.Katanas.Where(p => p.Title.ToLower().Contains(title.ToLower()));
        }
        public Order GetLastUnpayedOrder(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Manufacturer>> GetManufacturers()
        {
            return await _context.Manufacturers.ToListAsync();
        }
    }
}
