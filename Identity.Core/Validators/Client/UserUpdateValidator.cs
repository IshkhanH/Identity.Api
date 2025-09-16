using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Identity.Core.Models.User;

namespace Identity.Core.Validators.Client
{
    public class UserUpdateValidator : AbstractValidator<UserUpdate>
    {
        public UserUpdateValidator()
        {
            RuleFor(i => i.Email).NotEmpty().EmailAddress();
            RuleFor(i => i.Password).NotEmpty().Matches(@"^(?=.*[A-Za-z])(?=.*\d).+$")
                                    .MinimumLength(8)
                                    .WithMessage("Password-ը պետք է պարունակի տառեր թվեր" +
                                                 " և սիմվոլ և ոչ պակաս քան 8 նիշը");
        }
    }
}
