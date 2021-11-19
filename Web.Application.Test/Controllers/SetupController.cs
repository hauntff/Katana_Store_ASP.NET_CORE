using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Data;

namespace Web.Application.Test.Controllers
{
    public class SetupController : Controller
    {
        private readonly KatanaDbContext _context;
        private readonly ILogger<SetupController> _logger;
        public SetupController(
            ILogger<SetupController> logger,
            KatanaDbContext context)
        {
            _context = context;
            _logger = logger;
        }
        // GET: Setup
        public ActionResult Index()
        {
            ViewBag.IsEmptyManufacturers = !_context.Manufacturers.Any();
            ViewBag.IsEmptyCategories = !_context.Categories.Any();
            ViewBag.IsEmptyKatanas = !_context.Katanas.Any();
            ViewBag.IsEmptyPrices = !_context.Prices.Any();
            return View();
        }
        //[HttpPost]
        //public ActionResult Index(string create_manufacturers, string create_categories, string create_katanas, string create_prices)
        //{
        //    if (!string.IsNullOrWhiteSpace(create_manufacturers))
        //    {
        //        _context.Manufacturers.Add(new Domain.Manufacturer { Country = "Japon" });
        //        _context.Manufacturers.Add(new Domain.Manufacturer { Country = "USA" });
        //        _context.Manufacturers.Add(new Domain.Manufacturer { Country = "Russia" });
        //        _logger.LogInformation("added 3 manufacturers");
        //    }
        //    if (!string.IsNullOrWhiteSpace(create_categories))
        //    {
        //        _context.Categories.Add(new Category { NameOfCategory = "Metal" });
        //        _context.Categories.Add(new Category { NameOfCategory = "Metal" });
        //        _context.Categories.Add(new Category { NameOfCategory = "Wood" });
        //        _logger.LogInformation("added 3 categories");
        //    }
        //    if (!string.IsNullOrWhiteSpace(create_katanas))
        //    {
        //        _context.Katanas.Add(new Katana { Title = "Murasame" });
        //        _context.Katanas.Add(new Katana { Title = "Susanoo" });
        //        _context.Katanas.Add(new Katana { Title = "FirstBlood" });
        //        _logger.LogInformation("added 3 katanas");
        //    }
        //    var ok = SafeSave();
        //    if (ok != "ok")
        //        ViewBag.Error = ok;
        //    else
        //    {
        //        if (!string.IsNullOrWhiteSpace(create_prices))
        //        {
        //            var katana1 = _context.Katanas.FirstOrDefault(p => p.Title == "Murasame");
        //            var katana2 = _context.Katanas.FirstOrDefault(p => p.Title == "Susanoo");
        //            _context.Prices.Add(new Price { Katana = katana1, Amount = 10000, Created = DateTime.Now });
        //            _context.Prices.Add(new Price { Katana = katana2, Amount = 15000, Created = DateTime.Now.AddMonths(-1) });
        //            _context.Prices.Add(new Price { Katana = katana2, Amount = 25000, Created = DateTime.Now });
        //            _logger.LogInformation("added 3 prices");
        //        }
        //        SafeSave();
        //    }
        //    ViewBag.IsEmptyManufacturers = !_context.Manufacturers.Any();
        //    ViewBag.IsEmptyCategories = !_context.Categories.Any();
        //    ViewBag.IsEmptyKatanas = !_context.Katanas.Any();
        //    ViewBag.IsEmptyPrices = !_context.Prices.Any();
        //    return View();
        //}
        private string SafeSave()
        {
            try
            {
                _context.SaveChanges();
                return "ok";
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
                return exception.Message;
            }

        }
    }
}
