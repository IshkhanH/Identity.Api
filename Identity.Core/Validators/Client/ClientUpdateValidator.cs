using FluentValidation;
using Identity.Core.Models.Client;

namespace Identity.Core.Validators.Client
{
    public class ClientUpdateValidator : AbstractValidator<ClientUpdate>
    {
        public ClientUpdateValidator()
        {
            RuleFor(i => i.FirstName).NotEmpty().WithMessage("Անունը լրացված չէ:")
                                      .MinimumLength(3).WithMessage
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

            RuleFor(i => i.Documents).NotEmpty().WithMessage
                                                        ("Անձը հաստատող փաստաթուղթ մուտքագրված չէ:");
            RuleForEach(i => i.Documents).ChildRules(doc =>
            {
                doc.RuleFor(d => d)
                    .Must(d => !string.IsNullOrWhiteSpace(d.Passport) || !string.IsNullOrWhiteSpace(d.IdCard))
                    .WithMessage("Պարտադիր է լրացնել կամ Passport, կամ IdCard դաշտը:");

                doc.When(d => !string.IsNullOrWhiteSpace(d.Passport), () =>
                {
                    doc.RuleFor(d => d.Passport)
                        .MinimumLength(9).WithMessage("Passport համարը չպետք է 9 նիշից քիչ լինի:")
                        .MaximumLength(9).WithMessage("Passport համարը չպետք է գերազանցի 9 նիշը:");
                });

                doc.When(d => !string.IsNullOrWhiteSpace(d.IdCard), () =>
                {
                    doc.RuleFor(d => d.IdCard)
                        .MinimumLength(9).WithMessage("IdCard համարը չպետք է 9 նիշից քիչ լինի:")
                        .MaximumLength(9).WithMessage("IdCard համարը չպետք է գերազանցի 9 նիշը:");
                });
            });

            RuleFor(i => i.Emails).NotEmpty().WithMessage("Էլեկտրոնային հասցե մուտքագրված չէ:");

            RuleForEach(i => i.Emails).ChildRules(email =>
            {
                email.RuleFor(e => e.Email).EmailAddress().WithMessage("Էլեկտրոնային հասցեն վավեր չէ:");
            });

            RuleFor(i => i.Phones).NotEmpty().WithMessage("Հեռախոսահամար մուտքագրված չէ:");

            RuleForEach(i => i.Phones).ChildRules(phone =>
            {
                phone.RuleFor(p => p.PhoneNumber).MinimumLength(8).WithMessage
                ("Հեռախոսահամարը չի կարող եք պակաս լինել 8 նիշից").Matches(@"^[0-9+\-() ]+$").WithMessage
                ("Հեռախոսահամար դաշտում կարող եք մուտքագրել միայն սիմվոլ և թվեր:");
            });

            RuleFor(i => i.Address).NotEmpty().WithMessage("Հասցեն մուտքագրված չէ");
        }
    }

}
