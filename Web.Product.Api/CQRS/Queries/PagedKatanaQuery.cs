using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;
using Web.Product.Api.Interfaces;

namespace Web.Product.Api.CQRS.Queries
{
    public class PagedKatanaQuery : IRequest<IEnumerable<Katana>>, ICacheableQuery
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public bool BypassCache => false;
        public string CacheKey => $"book-{Page}-{PerPage}";

        public TimeSpan? SlidingExpiration => TimeSpan.FromMinutes(10);
    }
    public class PagedKatanaQueryHadler : IRequestHandler<PagedKatanaQuery, IEnumerable<Katana>>
    {
        private readonly IKatanaRepository _repository;
        public PagedKatanaQueryHadler(IKatanaRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Katana>> Handle(PagedKatanaQuery command, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_repository.GetPaged(command.Page, command.PerPage));
        }
    }
}

