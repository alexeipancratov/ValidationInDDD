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
        private readonly StateRepository _stateRepository;

        public StudentController(StudentRepository studentRepository, CourseRepository courseRepository,
            StateRepository stateRepository)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _stateRepository = stateRepository;
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            Address[] addresses = request.Addresses
                .Select(a => Address.Create(a.Street, a.City, a.State, a.ZipCode, _stateRepository.GetAll()).Value)
                .ToArray();
            var email = Email.Create(request.Email).Value;
            var studentName = request.Name.Trim(); // we can also trim it during validation.

            // NOTE: We violate the always-valid domain model guideline,
            // BUT we keep the domain model purity (i.e., not talking to external services) which is more important.ß
            Student existingStudent = _studentRepository.GetByEmail(email);
            if (existingStudent != null)
            {
                return Error(Errors.Student.EmailIsTaken());
            }

            // Accessing Value property would throw an exception if
            // validation failed. But we rely on FluentValidation here,
            // so if there was an error and we didn't catch it on the higher level
            // then we might have a bug in our system.
            var student = new Student(email, studentName, addresses);
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
            // var validator = new EditPersonalInfoRequestValidator();
            // var result = validator.Validate(request);
            //
            // if (!result.IsValid)
            // {
            //     return BadRequest(result.Errors[0].ErrorMessage);
            // }
            Student student = _studentRepository.GetById(id);
            if (student == null)
                return Error(Errors.General.NotFound(id), nameof(id));

            Address[] addresses = request.Addresses
                .Select(a => Address.Create(a.Street, a.City, a.State, a.ZipCode, _stateRepository.GetAll()).Value)
                .ToArray();
            // NOTE: We didn't create a Value Object for name (which would parse it as well)
            // because it has only one invariant (our guideline).
            string name = request.Name.Trim();
            
            student.EditPersonalInfo(name, addresses);
            _studentRepository.Save(student);

            return Ok();
        }

        [HttpPost("{id}/enrollments")]
        public IActionResult Enroll(long id, [FromBody] EnrollRequest request)
        {
            Student student = _studentRepository.GetById(id);
            if (student == null)
                return Error(Errors.General.NotFound(id), nameof(id));

            // NOTE: Here we traded Encapsulation of domain model again
            for (var i = 0; i < request.Enrollments.Length; i++)
            {
                CourseEnrollmentDto dto = request.Enrollments[i];

                string courseName = (dto.Course ?? string.Empty).Trim();
                Course course = _courseRepository.GetByName(courseName);
                if (course == null)
                    return Error(Errors.General.ValueIsInvalid(), $"{nameof(request.Enrollments)}[{i}].{dto.Course}");

                Grade grade = Grade.Create(dto.Grade).Value;

                var result = student.Enroll(course, grade);
                if (result.IsFailure)
                    return Error(result.Error);
            }

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            Student student = _studentRepository.GetById(id);

            var addresses = student.Addresses
                .Select(a => new AddressDto(a.Street, a.City, a.State.Value, a.ZipCode))
                .ToArray();
            var response = new GetResonse
            {
                Addresses = addresses,
                Email = student.Email.Value,
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
