using Microsoft.AspNetCore.Mvc;
using RegisterStudents.API.Controllers;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services;
using RegisterStudents.Tests.Helpers;
using System.Threading.Tasks;
using Xunit;
using RegisterStudents.API.Dto;

namespace RegisterStudents.Tests.Controllers
{
    public class StudentControllerTests
    {
        [Fact]
        public async Task AddStudent_ShouldReturnCreatedAtAction_WhenValidStudent()
        {
            var context = TestDbContextFactory.Create();
            var service = new StudentService(context);
            var controller = new StudentsController(service);

            var createDto = new StudentCreateDto
            {
                FirstName = "Laura",
                LastName = "Gomez",
                Email = "laura.gomez@example.com",
                Credits = 10,
                TeacherId = 1
            };

            var actionResult = await controller.CreateStudent(createDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdStudent = Assert.IsType<StudentReadDto>(createdAtActionResult.Value);
            Assert.Equal("Laura", createdStudent.FirstName);
        }

        [Fact]
        public async Task AddStudent_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            var context = TestDbContextFactory.Create();
            var service = new StudentService(context);
            var controller = new StudentsController(service);
            controller.ModelState.AddModelError("FirstName", "Required");

            var createDto = new StudentCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Credits = 10,
                TeacherId = 1
            };

            var result = await controller.CreateStudent(createDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task GetStudent_ShouldReturnStudent_WhenStudentExists()
        {
            var context = TestDbContextFactory.Create();
            var service = new StudentService(context);
            var controller = new StudentsController(service);

            var student = new Student
            {
                FirstName = "Anna",
                LastName = "Smith",
                Email = "anna.smith@example.com",
                Credits = 15,
                TeacherId = 2
            };

            context.Students.Add(student);
            await context.SaveChangesAsync();

            int studentId = student.Id;

            var result = await controller.GetStudent(studentId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var studentResult = Assert.IsType<StudentReadDto>(okResult.Value);
            Assert.Equal("Anna", studentResult.FirstName);
        }

        [Fact]
        public async Task GetStudent_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            var context = TestDbContextFactory.Create();
            var service = new StudentService(context);
            var controller = new StudentsController(service);

            var result = await controller.GetStudent(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateStudent_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            var context = TestDbContextFactory.Create();
            var service = new StudentService(context);
            var controller = new StudentsController(service);

            var student = new Student
            {
                FirstName = "Mark",
                LastName = "Johnson",
                Email = "mark.johnson@example.com",
                Credits = 12,
                TeacherId = 3
            };
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var updateDto = new StudentUpdateDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Credits = 20, // changed credits
                TeacherId = student.TeacherId
            };

            var result = await controller.UpdateStudent(student.Id, updateDto);

            Assert.IsType<NoContentResult>(result);

            var updatedStudent = await context.Students.FindAsync(student.Id);
            Assert.NotNull(updatedStudent);
            Assert.Equal(20, updatedStudent.Credits);
        }

        [Fact]
        public async Task UpdateStudent_ShouldReturnBadRequest_WhenIdMismatch()
        {
            var context = TestDbContextFactory.Create();
            var service = new StudentService(context);
            var controller = new StudentsController(service);

            var updateDto = new StudentUpdateDto
            {
                Id = 1,
                FirstName = "Tom",
                LastName = "Clark",
                Email = "tom.clark@example.com",
                Credits = 10,
                TeacherId = 1
            };

            var result = await controller.UpdateStudent(2, updateDto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeleteStudent_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            var context = TestDbContextFactory.Create();
            var service = new StudentService(context);
            var controller = new StudentsController(service);

            var student = new Student
            {
                FirstName = "Eva",
                LastName = "Brown",
                Email = "eva.brown@example.com",
                Credits = 14,
                TeacherId = 4
            };
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var result = await controller.DeleteStudent(student.Id);

            Assert.IsType<NoContentResult>(result);

            var deletedStudent = await context.Students.FindAsync(student.Id);
            Assert.Null(deletedStudent);
        }

        [Fact]
        public async Task DeleteStudent_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            var context = TestDbContextFactory.Create();
            var service = new StudentService(context);
            var controller = new StudentsController(service);

            var result = await controller.DeleteStudent(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
