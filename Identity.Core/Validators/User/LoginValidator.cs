using FluentValidation;
using Identity.Core.Models.User;

namespace Identity.Core.Validators.User
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(i => i.Email).NotEmpty().EmailAddress();
            RuleFor(i => i.Password).NotEmpty()
                                    .MinimumLength(8);
        }
    }
}
