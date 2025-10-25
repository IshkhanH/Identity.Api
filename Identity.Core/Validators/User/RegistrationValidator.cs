using FluentValidation;
using Identity.Core.Models.User;

namespace Identity.Core.Validators.User
{
    public class RegistrationValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationValidator()
        {
            RuleFor(i => i.FirstName).NotEmpty().WithMessage("Անունը լրացված չէ:")
                                     .MinimumLength(3). WithMessage
                ("Անունը չի կարող պակաս լինել 3 տառից").MaximumLength(100).WithMessage
                ("Անունը չի կարող գերազանցել 100 տառը").Matches(@"^[\p{L}]+$")
                                     .WithMessage("Անվան մեջ կարող են լինել միայն տառեր:");

            RuleFor(i => i.LastName).NotEmpty().WithMessage("Ազգանունը լրացված չէ:")
                                    .MinimumLength(3).WithMessage
                ("Ազգանունը չի կարող պակաս լինել 3 տառից").MaximumLength(100).WithMessage
                ("Ազգանունը չի կարող գերազանցել 100 տառը").Matches(@"^[\p{L}]+$")
                                    .WithMessage("Ազգանվան մեջ կարող են լինել միայն տառեր:");

            RuleFor(i => i.SurName).NotEmpty().WithMessage("Հայրանունը լրացված չէ:")
                                    .MinimumLength(3).WithMessage
                ("Հայրանունը չի կարող պակաս լինել 3 տառից").MaximumLength(100).WithMessage
                ("Հայրանոնը չի կարող գերազանցել 100 տառը").Matches(@"^[\p{L}]+$")
                                    .WithMessage("Հայրանվան մեջ կարող են լինել միայն տառեր:");

            RuleFor(i => i.Email).NotEmpty().WithMessage("էլեկտրոնային հասցեն լրացված չէ:")
                                 .EmailAddress().WithMessage("Էլեկտրոնային հասցեն վավեր չէ:");

            RuleFor(i => i.Password)
                   .NotEmpty().WithMessage("Գաղտնաբառը լրացված չէ:")
                   .MinimumLength(8).WithMessage("Գաղտնաբառը պետք է լինի առնվազն 8 նիշ.")
                   .Matches(@"[A-Za-z]").WithMessage("Գաղտնաբառը պետք է պարունակի առնվազն մեկ տառ.")
                   .Matches(@"\d").WithMessage("Գաղտնաբառը պետք է պարունակի առնվազն մեկ թիվ.")
                   .Matches(@"[!@#$%^&*(),.?""':{}|<>]").WithMessage("Գաղտնաբառը պետք է պարունակի առնվազն մեկ հատուկ սիմվոլ.");

        }
    }
}
