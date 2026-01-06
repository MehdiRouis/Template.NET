/*using Template.Core.Domain.Errors.Common;

namespace Template.Api.Application.Common.Validation
{
    public sealed class CompositeRequestValidator<T> : IRequestValidator<T>
    {
        private readonly IEnumerable<IRequestRule<T>> _rules;

        public CompositeRequestValidator(IEnumerable<IRequestRule<T>> rules)
        {
            _rules = rules;
        }

        public ValidationResult Validate(T request)
        {
            var errors = new List<DomainError>();

            foreach (var rule in _rules)
            {
                var result = rule.Validate(request);
                if (!result.IsValid)
                    errors.AddRange(result.Errors);
            }

            return ValidationResult.From(errors);
        }
    }
}*/
