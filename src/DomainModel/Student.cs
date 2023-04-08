using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using DomainModel.ValueObjects;

namespace DomainModel
{
    public class Student : Entity
    {
        public Email Email { get; }
        public string Name { get; private set; }
        public Address[] Addresses { get; private set; }

        private readonly List<Enrollment> _enrollments = new();
        public virtual IReadOnlyList<Enrollment> Enrollments => _enrollments.ToList();

        protected Student()
        {
        }

        public Student(Email email, string name, Address[] addresses)
            : this()
        {
            Email = email;
            EditPersonalInfo(name, addresses);
        }

        public void EditPersonalInfo(string name, Address[] addresses)
        {
            Name = name;
            Addresses = addresses;
        }

        // NOTE: Again, we could split it into two operations here - CanEnroll and Enroll (which would throw is CanEnroll
        // returns false) since here we don't parse input. This approach doesn't work when you parse input.
        public virtual UnitResult<Error> Enroll(Course course, Grade grade)
        {
            if (_enrollments.Count >= 2)
                return UnitResult.Failure(Errors.Student.TooManyEnrollments());

            if (_enrollments.Any(x => x.Course == course))
                return UnitResult.Failure(Errors.Student.AlreadyEnrolled(course.Name));

            var enrollment = new Enrollment(this, course, grade);
            _enrollments.Add(enrollment);

            return UnitResult.Success<Error>();
        }
    }
}
