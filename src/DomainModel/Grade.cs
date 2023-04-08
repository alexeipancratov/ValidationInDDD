using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;

namespace DomainModel
{
    public class Grade : ValueObject
    {
        // Possible values.
        public static Grade A = new(nameof(A));
        public static Grade B = new(nameof(B));
        public static Grade C = new(nameof(C));
        public static Grade D = new(nameof(D));
        public static Grade F = new(nameof(F));

        private static readonly Grade[] _all = { A, B, C, D, F };
        
        public string Value { get; }

        private Grade(string value)
        {
            Value = value;
        }

        public static Result<Grade, Error> Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Errors.General.ValueIsRequired();

            string grade = input.Trim().ToUpper();

            if (grade.Length > 1)
                return Errors.General.InvalidLength();

            if (_all.All(g => g.Value != grade))
            {
                return Errors.General.ValueIsInvalid();
            }

            return new Grade(grade);
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
