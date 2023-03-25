using System.Collections.Generic;
using System.Linq;
using DomainModel;
using DomainModel.ValueObjects;

namespace Api
{
    public class StudentRepository
    {
        private static readonly List<Student> ExistingStudents = new()
        {
            Alice(),
            Bob()
        };
        private static long _lastId = ExistingStudents.Max(x => x.Id);

        public Student GetById(long id)
        {
            // Retrieving from the database
            return ExistingStudents.SingleOrDefault(x => x.Id == id);
        }

        public void Save(Student student)
        {
            // Setting up the id for new students emulates the ORM behavior
            if (student.Id == 0)
            {
                _lastId++;
                SetId(student, _lastId);
            }

            // Saving to the database
            ExistingStudents.RemoveAll(x => x.Id == student.Id);
            ExistingStudents.Add(student);
        }

        private static void SetId(Entity entity, long id)
        {
            // The use of reflection to set up the Id emulates the ORM behavior
            entity.GetType().GetProperty(nameof(Entity.Id)).SetValue(entity, id);
        }

        private static Student Alice()
        {
            var addresses = new[]
            {
                new Address
                {
                    Street = "1234 Main St",
                    City = "Arlington",
                    State = "VA",
                    ZipCode = "22201"
                }
            };
            var email = Email.Create("alice@gmail.com");
            var name = "Alice Alison";
            var alice = new Student(email.Value, name, addresses);
            SetId(alice, 1);
            alice.Enroll(new Course(1, "Calculus", 5), Grade.A);

            return alice;
        }

        private static Student Bob()
        {
            var addresses = new[]
            {
                new Address
                {
                    Street = "2345 Second St",
                    City = "Barlington",
                    State = "VA",
                    ZipCode = "22202"
                }
            };
            var email = Email.Create("bob@gmail.com");
            var name = "Bob Bobson";
            var bob = new Student(email.Value, name, addresses);
            SetId(bob, 2);
            bob.Enroll(new Course(2, "History", 4), Grade.B);
            
            return bob;
        }
    }
}
