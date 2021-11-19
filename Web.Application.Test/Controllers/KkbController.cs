using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Store.Books.Domain;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Domain.Configs;
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;
using Web.Application.Infrastructure.Repo;
using Web.Application.Domain.Kkb;

namespace Web.Application.Test.Controllers
{
    public class KkbController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStoreService _service;
        private readonly KkbConfig _config;
        private readonly IKkbProtocolService _kkbService;
        public KkbController(
            IStoreService service,
            IOptions<KkbConfig> config,
            IKkbProtocolService kkbService,
            ILogger<HomeController> logger)
        {
            _kkbService = kkbService;
            _config = config?.Value;
            _logger = logger;
            _service = service;
        }
        // GET: KkbController
        [HttpPost]
        public async Task<ActionResult> Buy(int orderid)
        {

            var payment = _service.FindOrder(orderid);
            var sign = _kkbService.Build64Sync(payment.Id.ToString(), payment.Total);
            ViewBag.config = _config;
            return View(new KkbRequest
            {
                Email = payment.UserName,
                Sign = sign,
            });


        }
    }
}
