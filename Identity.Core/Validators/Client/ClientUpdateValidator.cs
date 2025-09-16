using FluentValidation;
using Identity.Core.Models.Client;

namespace Identity.Core.Validators.Client
{
    public class ClientUpdateValidator : AbstractValidator<ClientUpdate>
    {
        public ClientUpdateValidator()
        {
            RuleFor(i => i.FirstName).NotEmpty().MinimumLength(3).MaximumLength(100);
            RuleFor(i => i.LastName).NotEmpty().MinimumLength(3).MaximumLength(100);
            RuleFor(i => i.SurName).NotEmpty().MinimumLength(3).MaximumLength(100);
            RuleFor(i => i.Passport).NotEmpty().MinimumLength(10).MaximumLength(10);
            RuleFor(i => i.Mail).NotEmpty().EmailAddress();
            RuleFor(i => i.PhoneNumber).NotEmpty();
        }
    }
}
