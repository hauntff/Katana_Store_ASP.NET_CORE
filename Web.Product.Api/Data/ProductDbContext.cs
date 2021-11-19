using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Domain;

namespace Web.Product.Api.Data
{
    public class ProductDbContext : DbContext
    {
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Katana> Katanas { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<KatanaManufacturer> KatanaManufacturers { get; set; }
        public DbSet<KatanaCategory> KatanaCategories { get; set; }
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        // Конструктор для разработки, остальные конструкторы отключить
        //public ProductDbContext() { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          => optionsBuilder.UseNpgsql("Host = localhost; Port=5432;Database=KatanaStoreProduct;Username=postgres;Password=gangoptimus");
    }
}
