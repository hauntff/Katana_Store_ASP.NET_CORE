using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Application.Domain;

namespace Web.Application.Infrastructure.Data
{
    public class KatanaDbContext : DbContext
    {
        public DbSet<Katana> Katanas { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<KatanaManufacturer> KatanaManufacturers { get; set; }
        public DbSet<KatanaCategory> KatanaCategories { get; set; }
        public DbSet<Busket> Buskets { get; set; }
        public DbSet<Order> Orders{ get; set; }
        public DbSet<Price> Prices { get; set; }
        public KatanaDbContext(DbContextOptions<KatanaDbContext> options) : base(options) { }
       // public KatanaDbContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=KatanaStoreProduct;Username=postgres;Password=gangoptimus");

    }
}
