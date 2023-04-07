using DomainModel;
using DomainModel.ValueObjects;
using FluentValidation;

namespace Api.Validators;

public class AddressesValidator : AbstractValidator<AddressDto[]>
{
    public AddressesValidator(StateRepository stateRepository)
    {
        RuleFor(a => a)
            // .Must(a => a?.Length is >= 1 and <= 3)
            // .WithMessage("The number of messages must be between 1 and 3.")
            .ListMustContainNumberOfItems(1, 3)
            .ForEach(ar =>
            {
                ar.NotNull();
                // NOTE: With this validation we were violating some pillars of validation:
                // - validation rules should reside in domain layer
                // - no parsing support (i.e. we cannot trim values here for example, and persist this)
                // r.SetValidator(new AddressValidator());
                
                // NOTE: Initially, when we had state as a string.
                // ar.MustBeEntity(a => Address.Create(a.Street, a.City, a.State, a.ZipCode));
                ar.ChildRules(aValidator =>
                {
                    // No need to check next rule, if the State is invalid.
                    aValidator.ClassLevelCascadeMode = CascadeMode.Stop;
                    // If state is invalid then the error will be Addresses[0].State: "error message"
                    // so each field should report its state separately. We don't want to have Addresses[0]: "state is invalid"
                    aValidator
                        .RuleFor(a => a.State)
                        .MustBeValueObject(s => State.Create(s, stateRepository.GetAll()));
                    aValidator
                        .RuleFor(a => a)
                        .MustBeEntity(a => Address.Create(a.Street, a.City, a.State, a.ZipCode, stateRepository.GetAll()));
                });
            });
    }
}