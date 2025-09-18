using FluentValidation;
using Identity.Core.Models.User;

namespace Identity.Core.Validators.User
{
    public class RegistrationValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationValidator()
        {
            RuleFor(i => i.Email).NotEmpty().EmailAddress();
            RuleFor(i => i.Password).NotEmpty()
                                    .MinimumLength(8);
        }
    }
}
