using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DomainModel;
using FluentValidation;
using Entity = DomainModel.Entity;

namespace Api.Validators;

public static class CustomValidators
{
    public static IRuleBuilderOptions<T, TProperty> NotEmpty<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
    {
        // We want to use our own error messages which contain error code + separator + message itself.
        // TODO: Overload WithMessage to accept Error object.
        return DefaultValidatorExtensions.NotEmpty(ruleBuilder)
            .WithMessage(Errors.General.ValueIsRequired().Serialize());
    }

    public static IRuleBuilderOptions<T, string> Length<T>(this IRuleBuilder<T, string> ruleBuilder, int min, int max)
    {
        return DefaultValidatorExtensions.Length(ruleBuilder, min, max)
            .WithMessage(Errors.General.InvalidLength().Serialize());
    }
    
    public static IRuleBuilderOptions<T, TElement> MustBeEntity<T, TElement, TEntity>(
        this IRuleBuilder<T, TElement> ruleBuilder, Func<TElement, Result<TEntity, Error>> factoryMethod)
        where TEntity : Entity
    {
        return (IRuleBuilderOptions<T, TElement>)ruleBuilder.Custom((value, context) =>
        {
            Result<TEntity, Error> result = factoryMethod(value);

            if (result.IsFailure)
            {
                // TODO: Overload AddFailure to add Error objects instead.
                context.AddFailure(result.Error.Serialize());
            }
        });
    }
    
    public static IRuleBuilderOptions<T, string> MustBeValueObject<T, TValueObject>(
        this IRuleBuilder<T, string> ruleBuilder, Func<string, Result<TValueObject, Error>> factoryMethod)
        where TValueObject : ValueObject
    {
        return (IRuleBuilderOptions<T, string>)ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject, Error> result = factoryMethod(value);

            if (result.IsFailure)
            {
                context.AddFailure(result.Error.Serialize());
            }
        });
    }
    
    public static IRuleBuilderOptionsConditions<T, IList<TElement>> ListMustContainNumberOfItems<T, TElement>(
        this IRuleBuilder<T, IList<TElement>> ruleBuilder, int? min = null, int? max = null)
    {
        return ruleBuilder.Custom((list, context) =>
        {
            if (min.HasValue && list.Count < min.Value)
            {
                context.AddFailure(Errors.General.CollectionIsTooSmall(min.Value, list.Count).Serialize());
            }

            if (max.HasValue && list.Count > max.Value)
            {
                context.AddFailure(Errors.General.CollectionIsTooLarge(max.Value, list.Count).Serialize());
            }
        });
    }
}