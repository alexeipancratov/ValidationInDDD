using FluentValidation;

namespace Api;

public class EditPersonalInfoRequestValidator : AbstractValidator<EditPersonalInfoRequest>
{
    public EditPersonalInfoRequestValidator(StateRepository stateRepository)
    {
        RuleFor(r => r.Name).NotEmpty().Length(0, 200);
        RuleFor(r => r.Addresses).NotNull().SetValidator(new AddressesValidator(stateRepository));
    }
}