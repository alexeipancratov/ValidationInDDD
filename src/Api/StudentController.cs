using System;
using System.Linq;
using DomainModel;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Api
{
    [Route("api/students")]
    public class StudentController : Controller
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
            var validator = new RegisterRequestValidator();
            ValidationResult result = validator.Validate(request);

            if (!result.IsValid)
            {
                return BadRequest(result.Errors[0].ErrorMessage);
            }

            var address = new Address
            {
                City = request.Address.City,
                Street = request.Address.Street,
                State = request.Address.State,
                ZipCode = request.Address.ZipCode
            };
            var student = new Student(request.Email, request.Name, address);
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

            var address = new Address
            {
                City = request.Address.City,
                Street = request.Address.Street,
                State = request.Address.State,
                ZipCode = request.Address.ZipCode
            };
            student.EditPersonalInfo(request.Name, address);
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

            var address = new AddressDto
            {
                City = student.Address.City,
                Street = student.Address.Street,
                State = student.Address.State,
                ZipCode = student.Address.ZipCode
            };
            var response = new GetResonse
            {
                Address = address,
                Email = student.Email,
                Name = student.Name,
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
