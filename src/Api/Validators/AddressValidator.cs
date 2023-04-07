using FluentValidation;

namespace Api.Validators;

public class AddressValidator : AbstractValidator<AddressDto>
{
    public AddressValidator()
    {
        RuleFor(a => a.City).NotEmpty().Length(0, 40);
        RuleFor(a => a.Street).NotEmpty().Length(0, 100);
        RuleFor(a => a.State).NotEmpty().Length(0, 2);
        RuleFor(a => a.ZipCode).NotEmpty().Length(0, 5);
    }
}