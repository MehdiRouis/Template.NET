using Template.Core.Domain.Errors.Common;

namespace Template.Api.Application.Common.Validation;

public sealed class CompositeRequestValidator<T> : IRequestValidator<T>
{
    private readonly IEnumerable<IRequestValidator<T>> _validators;

    public CompositeRequestValidator(IEnumerable<IRequestValidator<T>> validators)
    {
        _validators = validators.Where(v => v.GetType() != typeof(CompositeRequestValidator<T>));
    }

    public ValidationResult Validate(T request)
    {
        var errors = new List<DomainError>();

        foreach (var v in _validators)
        {
            var result = v.Validate(request);
            if (!result.IsValid)
                errors.AddRange(result.Errors);
        }

        return ValidationResult.From(errors);
    }
}
