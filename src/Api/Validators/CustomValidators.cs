using System.Collections.Generic;
using FluentValidation;

namespace Api.Validators;

public static class CustomValidators
{
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