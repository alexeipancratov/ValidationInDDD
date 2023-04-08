using DomainModel;
using FluentValidation;

namespace Api.Validators;

public class EnrollRequestValidator : AbstractValidator<EnrollRequest>
{
    public EnrollRequestValidator()
    {
        RuleFor(r => r.Enrollments)
            .NotEmpty()
            .ListMustContainNumberOfItems(min: 1)
            .ForEach(er =>
            {
                er.NotNull();
                er.ChildRules(e =>
                {
                    e.RuleFor(ce => ce.Course).NotEmpty().Length(0, 100);
                    e.RuleFor(ce => ce.Grade).MustBeValueObject(Grade.Create);
                });
            });
    }
}