using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Application.Domain;
using Web.Application.Infrastructure.Interfaces;

namespace Web.Product.Api.CQRS.Commands
{
    public class CreateKatanaCommand : IRequest<bool>
    {
        public string Title { get; set; }
        public decimal Lenght { get; set; }
        public string Color { get; set; }
    }
    public class CreateKatanaCommandHandler : IRequestHandler<CreateKatanaCommand, bool>
    {
        private readonly IKatanaRepository _repository;
        public CreateKatanaCommandHandler(IKatanaRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(CreateKatanaCommand command, CancellationToken cancellationToken)
        {
            await _repository.Insert(new Katana
            {
                Title = command.Title,
                Lenght = command.Lenght,
                Color = command.Color
            });
            return true;
        }
    }
}
