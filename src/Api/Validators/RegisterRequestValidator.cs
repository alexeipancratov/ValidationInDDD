using System.Text.RegularExpressions;
using FluentValidation;

namespace Api;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(r => r.Name).NotEmpty().Length(0, 200);
        RuleFor(r => r.Addresses).NotNull().SetValidator(new AddressesValidator());
        
        // Conditional validation.
        // RuleFor(r => r.Phone)
        //     .NotEmpty()
        //     .Must(p => Regex.IsMatch(p, "^[2-9][0-9]{9}$"))
        //     // applies check only to the previous validation so that it doesn't fail.
        //     .When(r => r.Phone != null, ApplyConditionTo.CurrentValidator);

        // Only one contact method can be provided (phone/email).
        // NOTE: The first step of validation could've been achieved using
        // RuleFor too, but it would refer to multiple props in one rule chain.
        // So it's a bit messy.
        When(r => r.Email == null, () =>
        {
            RuleFor(r => r.Phone).NotEmpty();
        });
        When(r => r.Phone == null, () =>
        {
            RuleFor(r => r.Email).NotEmpty();
        });
        
        RuleFor(r => r.Email)
            .NotEmpty()
            .Length(0, 150)
            .EmailAddress()
            .When(r => r.Email != null);
        
        RuleFor(r => r.Phone)
            .NotEmpty()
            .Matches("^[2-9][0-9]{9}$")
            .When(r => r.Phone != null);
    }
}