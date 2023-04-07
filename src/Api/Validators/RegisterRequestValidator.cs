using DomainModel.ValueObjects;
using FluentValidation;

namespace Api.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator(StateRepository stateRepository)
    {
        // We can control when to stop validation.
        // Can also be configured in each rule set separately as well.
        // ClassLevelCascadeMode = CascadeMode.Stop;
        // RuleLevelCascadeMode = CascadeMode.Stop;
        
        // NOTE: We could transform name here as well (by trimming it), during validation
        // Using the Transform method, but it was deprecated.
        // Using a computed property is recommended instead.
        // BUT it would make our validation more complicated.
        // After all we're still validating if length is more than 200 chars (and trim it afterwards in the controller).
        // But if users sent data with extra spaces he/she will simply have less available chars.

        RuleFor(r => r.Name).NotEmpty().Length(0, 200);
        RuleFor(r => r.Addresses).NotNull().SetValidator(new AddressesValidator(stateRepository));
        
        // Conditional validation.
        // NOTE: These validations will be run sequentially by default regardless of failures, so Must
        // will be invoked together with NotEmpty.
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
        
        // RuleFor(r => r.Email)
        //     .NotEmpty()
        //     .Length(0, 150)
        //     .EmailAddress()
        //     .When(r => r.Email != null);

        RuleFor(r => r.Email)
            .MustBeValueObject(Email.Create)
            .When(r => r.Email != null);
        
        RuleFor(r => r.Phone)
            .NotEmpty()
            .Matches("^[2-9][0-9]{9}$")
            .When(r => r.Phone != null);
    }
}