using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;

namespace Web.Product.Api.CQRS.Queries
{
    public class KatanaQuery : IRequest<IEnumerable<Katana>>
    {
        public string Title { get; set; }
    }
    public class BookQueryHandler : IRequestHandler<KatanaQuery, IEnumerable<Katana>>
    {
        private readonly IKatanaRepository _repository;

        public BookQueryHandler(IKatanaRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Katana>> Handle(KatanaQuery request, CancellationToken cancellationToken)
        {
            return await _repository.Get(p => p.Title == request.Title);
        }
    }
}
