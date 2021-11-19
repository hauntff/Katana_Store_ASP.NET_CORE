using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Product.Api.CQRS.Commands;

namespace Web.Product.Api.Validators
{
    public class CreateKatanaCommandValidator : AbstractValidator<CreateKatanaCommand>
    {
        public CreateKatanaCommandValidator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.Title).MinimumLength(3);
            RuleFor(c => c.Title).MaximumLength(500);
        }
    }
}
