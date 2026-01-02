namespace Template.Api.Application.Common.Validation;

public interface IRequestValidator<T>
{
    ValidationResult Validate(T request);
}
