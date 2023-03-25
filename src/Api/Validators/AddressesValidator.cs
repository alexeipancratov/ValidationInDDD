using Api.Validators;
using DomainModel;
using FluentValidation;

namespace Api;

public class AddressesValidator : AbstractValidator<AddressDto[]>
{
    public AddressesValidator()
    {
        RuleFor(a => a)
            // .Must(a => a?.Length is >= 1 and <= 3)
            // .WithMessage("The number of messages must be between 1 and 3.")
            .ListMustContainNumberOfItems(1, 3)
            .ForEach(r =>
            {
                r.NotNull();
                // NOTE: With this validation we were violating some pillars of validation:
                // - validation rules should reside in domain layer
                // - no parsing support
                // r.SetValidator(new AddressValidator());
                r.MustBeEntity(a => Address.Create(a.Street, a.City, a.State, a.ZipCode));
            });
    }
}