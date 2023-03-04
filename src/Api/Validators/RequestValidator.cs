using FluentValidation;

namespace Api;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().Length(0, 200);
        RuleFor(r => r.Email).NotEmpty().Length(0, 150).EmailAddress();

        RuleFor(r => r.Addresses).NotNull().SetValidator(new AddressesValidator());
    }
}