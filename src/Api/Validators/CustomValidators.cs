using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DomainModel;
using FluentValidation;
using Entity = DomainModel.Entity;

namespace Api.Validators;

public static class CustomValidators
{
    public static IRuleBuilderOptions<T, TElement> MustBeEntity<T, TElement, TEntity>(
        this IRuleBuilder<T, TElement> ruleBuilder, Func<TElement, Result<TEntity, Error>> factoryMethod)
        where TEntity : Entity
    {
        return (IRuleBuilderOptions<T, TElement>)ruleBuilder.Custom((value, context) =>
        {
            Result<TEntity, Error> result = factoryMethod(value);

            if (result.IsFailure)
            {
                context.AddFailure(result.Error.Message);
            }
        });
    }
    
    public static IRuleBuilderOptions<T, string> MustBeValueObject<T, TValueObject>(
        this IRuleBuilder<T, string> ruleBuilder, Func<string, Result<TValueObject, Error>> factoryMethod)
        where TValueObject : ValueObject
    {
        return (IRuleBuilderOptions<T, string>)ruleBuilder.Custom((value, context) =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            Result<TValueObject, Error> result = factoryMethod(value);

            if (result.IsFailure)
            {
                context.AddFailure(result.Error.Message);
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
                context.AddFailure($"The list must contain {min.Value} items or more. It contains {list.Count} items.");
            }

            if (max.HasValue && list.Count > max.Value)
            {
                context.AddFailure($"The list must contain {max.Value} items or more. It contains {list.Count} items.");
            }
        });
    }
}