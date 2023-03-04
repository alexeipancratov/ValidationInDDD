﻿using System.Collections.Generic;
using System.Linq;
using DomainModel;

namespace Api
{
    public class StudentRepository
    {
        private static readonly List<Student> _existingStudents = new()
        {
            Alice(),
            Bob()
        };
        private static long _lastId = _existingStudents.Max(x => x.Id);

        public Student GetById(long id)
        {
            // Retrieving from the database
            return _existingStudents.SingleOrDefault(x => x.Id == id);
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
            _existingStudents.RemoveAll(x => x.Id == student.Id);
            _existingStudents.Add(student);
        }

        private static void SetId(Entity entity, long id)
        {
            // The use of reflection to set up the Id emulates the ORM behavior
            entity.GetType().GetProperty(nameof(Entity.Id)).SetValue(entity, id);
        }

        private static Student Alice()
        {
            var address = new Address
            {
                Street = "1234 Main St",
                City = "Arlington",
                State = "VA",
                ZipCode = "22201"
            };
            var alice = new Student("alice@gmail.com", "Alice Alison", address);
            SetId(alice, 1);
            alice.Enroll(new Course(1, "Calculus", 5), Grade.A);

            return alice;
        }

        private static Student Bob()
        {
            var address = new Address
            {
                Street = "2345 Second St",
                City = "Barlington",
                State = "VA",
                ZipCode = "22202"
            };
            var bob = new Student("bob@gmail.com", "Bob Bobson", address);
            SetId(bob, 2);
            bob.Enroll(new Course(2, "History", 4), Grade.B);
            
            return bob;
        }
    }
}
