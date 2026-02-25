using FluentValidation;
using MediatR;
using ValidationException = CustomerService.Api.Features.Common.Exceptions.ValidationException;

namespace CustomerService.Api.Features.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validations;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validations)
    {
        _validations = validations;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validations.Any())
        {
            var validationResults = await Task.WhenAll(
                _validations
                    .Select(v => v.ValidateAsync(
                        new ValidationContext<TRequest>(request), cancellationToken)));
            
            var failures = validationResults
                .Where(v=>v.Errors.Any())
                .SelectMany(v => v.Errors)
                .ToList();
            if (failures.Count != 0)
                throw new ValidationException(failures);
        }
        return await next();
    }
}