using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Application.Infrastructure.Interfaces;
using Web.Product.Api.CQRS.Commands;
using Web.Product.Api.CQRS.Queries;

namespace Web.Product.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IKatanaRepository _katanaRepository;
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator,
            IKatanaRepository katanaRepository,
            ILogger<ProductController> logger)
        {
            _logger = logger;
            _katanaRepository = katanaRepository;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string title)
        {
            return Ok(await _mediator.Send(new KatanaQuery { Title = title }));
        }

        [HttpGet("byId")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await _katanaRepository.GetById(id));
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int page, int perPage)
        {
            return Ok(await _mediator.Send(new PagedKatanaQuery { Page = page, PerPage = perPage }));
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateKatanaCommand command, CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(command, cancellationToken));
        }
    }
}
