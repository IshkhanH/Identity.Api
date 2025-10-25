using FluentValidation;
using Identity.Core.Models.User;


namespace Identity.Core.Validators.User
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(i => i.Email).NotEmpty().WithMessage("էլեկտրոնային հասցեն լրացված չէ:");
                                 

            RuleFor(i => i.Password)
                  .NotEmpty().WithMessage("Գաղտնաբառը լրացված չէ:");
        }
    }
}
