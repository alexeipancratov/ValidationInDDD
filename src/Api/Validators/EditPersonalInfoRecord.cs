using FluentValidation;

namespace Api;

public class EditPersonalInfoRequestValidator : AbstractValidator<EditPersonalInfoRequest>
{
    public EditPersonalInfoRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().Length(0, 200);
        RuleFor(r => r.Address).NotNull().SetValidator(new AddressValidator());
    }
}