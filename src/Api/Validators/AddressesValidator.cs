using FluentValidation;

namespace Api;

public class AddressesValidator : AbstractValidator<AddressDto[]>
{
    public AddressesValidator()
    {
        RuleFor(a => a)
            .Must(a => a?.Length is >= 1 and <= 3)
            .WithMessage("The number of messages must be between 1 and 3.")
            .ForEach(r =>
            {
                r.NotNull();
                r.SetValidator(new AddressValidator());
            });
    }
}