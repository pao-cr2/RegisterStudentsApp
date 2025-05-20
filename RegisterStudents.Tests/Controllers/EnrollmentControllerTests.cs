using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegisterStudents.API.Controllers;
using RegisterStudents.API.Data;
using RegisterStudents.API.Models;
using RegisterStudents.Tests.Helpers;
using System.Threading.Tasks;
using Xunit;
using Moq;
using RegisterStudents.API.Services;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Services.Results;

namespace RegisterStudents.Tests.Controllers
{
    public class EnrollmentControllerTests
    {
        /*
         Input: Enrollment object with valid StudentId and SubjectId.
         Output: IActionResult of type CreatedAtActionResult containing the created Enrollment.
         Description: Ensures a valid enrollment returns CreatedAtActionResult with correct data.
        */
        [Fact]
        public async Task AddEnrollment_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            var mockService = new Mock<IEnrollmentService>();

            var enrollmentDto = new EnrollmentCreateDto
            {
                StudentId = 3,
                SubjectId = 2
            };

            var createdEnrollment = new Enrollment
            {
                Id = 1,
                StudentId = 3,
                SubjectId = 2
            };

            // Devuelve un ServiceResult<Enrollment> con éxito y la data
            var serviceResult = ServiceResult<Enrollment>.Success(createdEnrollment);

            mockService.Setup(s => s.CreateEnrollmentAsync(It.IsAny<EnrollmentCreateDto>()))
                       .ReturnsAsync(serviceResult);

            var controller = new EnrollmentsController(mockService.Object);

            // Act
            var result = await controller.PostEnrollment(enrollmentDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<EnrollmentReadDto>(createdResult.Value);
            Assert.Equal(enrollmentDto.StudentId, returnValue.StudentId);
            Assert.Equal(enrollmentDto.SubjectId, returnValue.SubjectId);
        }

        /*
          Input: Enrollment object missing required StudentId.
          Output: BadRequestObjectResult with validation errors.
          Description: Ensures BadRequest is returned when the model state is invalid.
         */
        [Fact]
        public async Task AddEnrollment_ShouldReturnBadRequest_WhenModelInvalid()
        {
            // Arrange
            var mockService = new Mock<IEnrollmentService>();
            var controller = new EnrollmentsController(mockService.Object);

            controller.ModelState.AddModelError("StudentId", "Required");

            var enrollmentDto = new EnrollmentCreateDto
            {
                SubjectId = 2
            };

            // Act
            var result = await controller.PostEnrollment(enrollmentDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }


        /*
         Input: Existing Enrollment ID.
         Output: OkObjectResult with the corresponding Enrollment.
         Description: Retrieves an existing enrollment by ID.
        */
        [Fact]
        public async Task GetEnrollment_ShouldReturnEnrollment_WhenExists()
        {
            // Arrange
            var context = TestDbContextFactory.Create();


            var teacher = new Teacher
            {
                Id = 1,
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice.smith@example.com" // Fix: Set the required 'Email' property
            };

            var student = new Student
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Credits = 10,
                TeacherId = 1
            };

            var subject = new Subject
            {
                Id = 2,
                Name = "Mathematics",
                Credits = 3,
                TeacherId = teacher.Id,
                Teacher = teacher
            };

            var enrollment = new Enrollment
            {
                Id = 1,
                StudentId = student.Id,
                SubjectId = subject.Id,
                Student = student,
                Subject = subject
            };

            context.Teachers.Add(teacher);
            context.Students.Add(student);
            context.Subjects.Add(subject);
            context.Enrollments.Add(enrollment);
            await context.SaveChangesAsync();

            var service = new EnrollmentService(context);
            var controller = new EnrollmentsController(service);

            // Act
            var result = await controller.GetEnrollment(enrollment.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var fetched = Assert.IsType<EnrollmentReadDto>(okResult.Value);
            Assert.Equal(enrollment.Id, fetched.Id);
            Assert.Equal("John", fetched.StudentFirstName);
            Assert.Equal("Mathematics", fetched.SubjectName);
        }

        /*
         Input: Non-existent Enrollment ID.
         Output: NotFoundResult.
         Description: Returns NotFound when the specified enrollment does not exist.
        */
        [Fact]
        public async Task GetEnrollment_ShouldReturnNotFound_WhenNotExists()
        {
            var context = TestDbContextFactory.Create();
            var service = new EnrollmentService(context);
            var controller = new EnrollmentsController(service);

            var result = await controller.GetEnrollment(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }


        /*
         Input: Existing Enrollment ID.
         Output: NoContentResult.
         Description: Deletes an existing enrollment and confirms with NoContent.
        */
        [Fact]
        public async Task DeleteEnrollment_ShouldReturnNoContent_WhenExists()
        {
            var mockService = new Mock<IEnrollmentService>();

            int enrollmentId = 1;

            // Devuelve ServiceResult<bool> indicando éxito
            mockService.Setup(s => s.DeleteEnrollmentAsync(enrollmentId))
                       .ReturnsAsync(ServiceResult<bool>.Success(true));

            var controller = new EnrollmentsController(mockService.Object);

            var result = await controller.DeleteEnrollment(enrollmentId);

            Assert.IsType<NoContentResult>(result);
        }


        /*
         Input: Non-existent Enrollment ID.
         Output: NotFoundResult.
         Description: Returns NotFound when trying to delete a non-existent enrollment.
        */
        [Fact]
        public async Task DeleteEnrollment_ShouldReturnNotFound_WhenNotExists()
        {
            var mockService = new Mock<IEnrollmentService>();

            int nonExistentEnrollmentId = 999;

            mockService.Setup(s => s.DeleteEnrollmentAsync(nonExistentEnrollmentId))
                       .ReturnsAsync(ServiceResult<bool>.Failure(new[] { "Enrollment not found" }));

            var controller = new EnrollmentsController(mockService.Object);

            var result = await controller.DeleteEnrollment(nonExistentEnrollmentId);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        /*
         Input: Non-existent Enrollment ID.
         Output: NotFoundResult.
         Description: Returns NotFound when trying to delete a non-existent enrollment.
        */
        [Fact]
        public async Task DeleteEnrollment_ShouldReturnNotFound_WhenEnrollmentDoesNotExist()
        {
            var mockService = new Mock<IEnrollmentService>();

            int enrollmentId = 999; // Non-existent enrollment ID
            mockService.Setup(s => s.DeleteEnrollmentAsync(enrollmentId))
                       .ReturnsAsync(ServiceResult<bool>.Failure(new[] { "Enrollment not found" }));

            var controller = new EnrollmentsController(mockService.Object);

            var result = await controller.DeleteEnrollment(enrollmentId);

            // Si tu controlador retorna NotFound() con BadRequest(result.Errors) u otro tipo,
            // Ajusta el assert según lo que retorna realmente el controlador:
            Assert.IsType<NotFoundObjectResult>(result);
        }

    }
}
