namespace DomainModel.ValueObjects;

// NOTE: It makes sense to create a Value Object to replace the primitive type
// only if there's more than one possible invariant.
// public class StudentName : ValueObject
// {
//     public string Value { get; }
//
//     private StudentName(string value)
//     {
//         Value = value;
//     }
//     
//     public static Result<StudentName, Error> Create(string input)
//     {
//         if (string.IsNullOrWhiteSpace(input))
//             return Errors.General.ValueIsRequired();
//
//         string email = input.Trim();
//
//         if (email.Length > 200)
//             return Errors.General.InvalidLength();
//
//         return new StudentName(email);
//     }
//     
//     protected override IEnumerable<IComparable> GetEqualityComponents()
//     {
//         yield return Value;
//     }
// }