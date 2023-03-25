using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DomainModel.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }
    
    // NOTE: Separating validation (into a separate Validate method) and object creation (into a ctor)
    // could result in logic duplication (for trimming in this case).
    // So it's better to have it all in one place.
    // Although there're some exception when:
    // - no validation is needed, so we can just init it in ctor
    // - no parsing is needed, so we could split it into two operations.
    public static Result<Email, Error> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Errors.General.ValueIsRequired();

        // NOTE: Validation is also - parsing of data (in some cases).
        string email = input.Trim();

        if (email.Length > 150)
            return Errors.General.InvalidLength();

        if (Regex.IsMatch(email, @"^(.+)@(.+)$") == false)
            return Errors.General.ValueIsInvalid();

        return new Email(email);
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}