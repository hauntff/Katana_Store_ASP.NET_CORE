using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Application.Domain;
using Web.Application.Infrastructure.Data;
using Web.Application.Infrastructure.Repo;

namespace Web.Application.Test.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStoreService _service;
        public OrderController(
            IStoreService service,
            ILogger<HomeController> logger)
        {
            _logger = logger;
            _service = service;
        }
        public async Task<ActionResult> Buy(int katanaId)
        {
            var katana = await _service.GetKatanaById(katanaId);
            return View();
        }
        public async Task<ActionResult> Bu1y(int katanaId)
        {
            var katana = await _service.GetKatanaById(katanaId);
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Buy(int katanaId, string userName)
        {
            var order = await _service.CreateOrUpdateOrder(katanaId, userName);
            return View(order);
        }

    }
}
