﻿using System;
using System.Linq;
using DomainModel;
using DomainModel.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Api
{
    [Route("api/students")]
    public class StudentController : ApplicationController
    {
        private readonly StudentRepository _studentRepository;
        private readonly CourseRepository _courseRepository;

        public StudentController(StudentRepository studentRepository, CourseRepository courseRepository)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var addresses = request.Addresses
                .Select(a => new Address(a.Street, a.City, a.State, a.ZipCode))
                .ToArray();
            var email = Email.Create(request.Email);
            var name = StudentName.Create(request.Name);
            if (email.IsFailure)
                return BadRequest(email.Error.Message);
            if (name.IsFailure)
                return BadRequest(name.Error.Message);
            
            var student = new Student(email.Value, name.Value, addresses);
            _studentRepository.Save(student);

            var response = new RegisterResponse
            {
                Id = student.Id
            };
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult EditPersonalInfo(long id, [FromBody] EditPersonalInfoRequest request)
        {
            var validator = new EditPersonalInfoRequestValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors[0].ErrorMessage);
            }
            
            Student student = _studentRepository.GetById(id);

            var addresses = request.Addresses
                .Select(a => new Address(a.Street, a.City, a.State, a.ZipCode))
                .ToArray();
            // student.EditPersonalInfo(request.Name, addresses);
            _studentRepository.Save(student);

            return Ok();
        }

        [HttpPost("{id}/enrollments")]
        public IActionResult Enroll(long id, [FromBody] EnrollRequest request)
        {
            Student student = _studentRepository.GetById(id);

            foreach (CourseEnrollmentDto enrollmentDto in request.Enrollments)
            {
                Course course = _courseRepository.GetByName(enrollmentDto.Course);
                var grade = Enum.Parse<Grade>(enrollmentDto.Grade);
                
                student.Enroll(course, grade);
            }

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            Student student = _studentRepository.GetById(id);

            var addresses = student.Addresses
                .Select(a => new AddressDto(a.Street, a.City, a.State, a.ZipCode))
                .ToArray();
            var response = new GetResonse
            {
                Addresses = addresses,
                Email = student.Email.Value,
                Name = student.Name.Value,
                Enrollments = student.Enrollments.Select(x => new CourseEnrollmentDto
                {
                    Course = x.Course.Name,
                    Grade = x.Grade.ToString()
                }).ToArray()
            };
            return Ok(response);
        }
    }
}
