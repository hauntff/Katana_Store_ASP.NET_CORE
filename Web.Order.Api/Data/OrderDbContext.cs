using Microsoft.EntityFrameworkCore;
using Store.Books.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Domain;

namespace Web.Order.Api.Data
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Busket> Buskets { get; set; }
        public DbSet<Application.Domain.Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
        //public OrderDbContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=KatanaStoreProduct;Username=postgres;Password=gangoptimus");
    }
}
