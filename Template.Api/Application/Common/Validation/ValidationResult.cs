using Template.Core.Domain.Errors.Common;

namespace Template.Api.Application.Common.Validation;

public sealed class ValidationResult
{
    public IReadOnlyList<DomainError> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    private ValidationResult(List<DomainError> errors)
    {
        Errors = errors;
    }

    public static ValidationResult From(List<DomainError> errors)
        => new ValidationResult(errors);
}
