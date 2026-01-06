using Template.Api.Application.Common.Validation;
using Template.Api.Models.Views.Requests.Authentication;
using Template.Core.Domain.Errors.Authentication;
using Template.Core.Domain.Errors.Common;
using Template.Core.Domain.Policies;

namespace Template.Api.Application.Authentication.Validators
{
    public sealed class SigninValidator : IRequestValidator<SigninRequest>
    {
        public ValidationResult Validate(SigninRequest r)
        {
            var errors = new List<DomainError>();

            if (!EmailPolicy.IsValid(r.Email) || !PasswordPolicy.IsValid(r.Password))
                errors.Add(SigninErrors.InvalidRequest);

            return ValidationResult.From(errors);
        }
    }

}
