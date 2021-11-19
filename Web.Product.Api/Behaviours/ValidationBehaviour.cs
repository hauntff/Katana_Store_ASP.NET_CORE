using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebPages;

namespace Web.Product.Api.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponce> : IPipelineBehavior<TRequest, TResponce>
        where TRequest : IRequest<TResponce>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponce> Handle(TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponce> next)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults =
                    await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(result => result.Errors)
                    .Where(error => error != null).ToList();
                if (failures.Any())
                {
                    throw new Exception(
                        $"Команда {typeof(TRequest).Name} вызвала ошибку при валидации",
                        new ValidationException("ОШибка валидации", failures));
                }
            }
            return await next();
        }
    }
}
