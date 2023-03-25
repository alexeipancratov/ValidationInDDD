using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace DomainModel.ValueObjects;

public class StudentName : ValueObject
{
    public string Value { get; }

    private StudentName(string value)
    {
        Value = value;
    }
    
    public static Result<StudentName, Error> Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Errors.General.ValueIsRequired();

        string email = input.Trim();

        if (email.Length > 200)
            return Errors.General.InvalidLength();

        return new StudentName(email);
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value;
    }
}