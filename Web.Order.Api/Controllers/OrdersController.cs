using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Books.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Order.Api.Interfaces;

namespace Web.Order.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IStoreService _service;
        public OrdersController(IStoreService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrder order)
        {
            var dbOrder = await _service.CreateOrUpdateOrder(order.KatanaId, order.Title, order.PriceId, order.Price, order.UserName);
            return Ok(dbOrder);
        }
    }
}
