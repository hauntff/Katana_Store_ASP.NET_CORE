using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Product.Api.CQRS.Queries;

namespace Web.Product.Api.Validators
{
    public class QueryValidator : AbstractValidator<PagedKatanaQuery>
    {
        public QueryValidator()
        {
            RuleFor(c => c.PerPage).GreaterThan(0);
            RuleFor(c => c.Page).GreaterThan(0);
            RuleFor(c => c.PerPage).LessThan(21);
        }
    }
}
