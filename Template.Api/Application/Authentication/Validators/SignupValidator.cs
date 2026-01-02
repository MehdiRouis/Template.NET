using Template.Api.Application.Common.Validation;
using Template.Api.Models.Views.Requests.Authentication;
using Template.Core.Domain.Errors.Authentication;
using Template.Core.Domain.Errors.Common;
using Template.Core.Domain.Policies;

namespace Template.Api.Application.Authentication.Validators
{
    public sealed class SignupValidator : IRequestValidator<SignupRequest>
    {
        public ValidationResult Validate(SignupRequest r)
        {
            var errors = new List<DomainError>();

            if (!EmailPolicy.IsValid(r.Email))
                errors.Add(SignupErrors.InvalidEmail);

            if (!PasswordPolicy.IsStrong(r.Password))
                errors.Add(SignupErrors.WeakPassword);

            if (!PhonePolicy.IsValid(r.Phone))
                errors.Add(SignupErrors.InvalidPhone);

            if (!NamePolicy.IsValid(r.FullName))
                errors.Add(SignupErrors.InvalidName);

            return ValidationResult.From(errors);
        }
    }

}
