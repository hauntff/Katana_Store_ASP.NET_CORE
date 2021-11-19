using ExcelDataReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;
using Web.Common.Configs;

namespace Web.Product.Api.Background
{
    public class TaskService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;
        private readonly ServiceConfig _config;
        private readonly IHttpClientFactory _factory;
        public TaskService(IServiceProvider provider,
            IHttpClientFactory factory,
            IOptions<ServiceConfig> config,
            ILogger<TaskService> logger)
        {
            _provider = provider;
            _logger = logger;
            _config = config?.Value;
            _factory = factory;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _provider.CreateScope();
            var katanas = scope.ServiceProvider.GetRequiredService<IKatanaRepository>();
            var prices = scope.ServiceProvider.GetRequiredService<IPriceRepository>();
            var manufacturers = scope.ServiceProvider.GetRequiredService<IManufacturerRepository>();
            var categories = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
            var katanamanufacturer = scope.ServiceProvider.GetRequiredService<IKatanaManufacturerRepository>();
            var katanacategory = scope.ServiceProvider.GetRequiredService<IKatanaCategoryRepository>();
            var orders = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            // var bookauthor = scope.ServiceProvider.GetRequiredService<IBook>();
            while (!stoppingToken.IsCancellationRequested)
            {
                using var stream = File.Open(_config.ExcelFileKatana, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                var counter = 0;
                do
                {
                    while (reader.Read())
                    {
                        if (counter == 0) { counter++; continue; }
                        var title = reader.GetString(0);
                        var lenght = reader.GetDouble(4);
                        var color = reader.GetString(5);
                        var lenghtInt = Convert.ToInt32(lenght);
                        var manufacturerExcel = reader.GetString(1);
                        var categoryExcel = reader.GetString(2);
                        var priceExcel = reader.GetDouble(3);
                        var username = reader.GetString(6);
                        var total = reader.GetDouble(7);
                        var totalPrice = Convert.ToDecimal(total);
                        var id = Guid.NewGuid().ToString();
                        var priceDouble = Convert.ToDouble(priceExcel);
                        var katana = (await katanas.Get(p => p.Title == title)).FirstOrDefault();
                        if (katana is null)
                        {
                            await katanas.Insert(new Katana { Title = title, Lenght = lenghtInt, Color = color, Code=id});
                            katana = (await katanas.Get(p => p.Title == title)).FirstOrDefault();
                        }

                        var manufacturer = (await manufacturers.Get(p => p.Country == manufacturerExcel)).FirstOrDefault();
                        if (manufacturer is null)
                        {
                            await manufacturers.Insert(new Manufacturer { Country = manufacturerExcel });
                            manufacturer = (await manufacturers.Get(p => p.Country == manufacturerExcel)).FirstOrDefault();
                        }

                        var price = (await prices.Get(p => p.Amount == Convert.ToDecimal(priceDouble))).FirstOrDefault();
                        if (price is null)
                        {
                            await prices.Insert(new Price { Amount = Convert.ToDecimal(priceDouble), Katana = katana, KatanaCode=id});
                            price = (await prices.Get(p => p.Amount == Convert.ToDecimal(priceDouble))).FirstOrDefault();
                        }

                        var category = (await categories.Get(p => p.NameOfCategory == categoryExcel)).FirstOrDefault();
                        if (category is null)
                        {
                            await categories.Insert(new Category { NameOfCategory = categoryExcel });
                            category = (await categories.Get(p => p.NameOfCategory == categoryExcel)).FirstOrDefault();
                        }

                        var order = (await orders.Get(p => p.UserName == username)).FirstOrDefault();
                        if (order is null)
                        {
                            await orders.Insert(new Order { UserName = username, Status=0, Total = totalPrice });
                            order = (await orders.Get(p => p.UserName == username)).FirstOrDefault();
                        }
                        await katanamanufacturer.Insert(new KatanaManufacturer { Katana = katana, Manufacturer = manufacturer });
                        await katanacategory.Insert(new KatanaCategory { Katana = katana, Category = category });
                    }
                } while (reader.NextResult());
                Thread.Sleep(10 * 1000);
            }
        }
    }
}
