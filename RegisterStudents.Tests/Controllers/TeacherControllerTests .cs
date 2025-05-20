using Microsoft.AspNetCore.Mvc;
using RegisterStudents.API.Controllers;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services;  // Your TeacherService namespace
using RegisterStudents.Tests.Helpers; // Your TestDbContextFactory namespace
using System.Threading.Tasks;
using Xunit;

namespace RegisterStudents.Tests.Controllers
{
    public class TeachersControllerTests
    {
        [Fact]
        public async Task AddTeacher_ShouldReturnCreatedAtAction_WhenValidTeacher()
        {
            var context = TestDbContextFactory.Create();
            var service = new TeacherService(context);
            var controller = new TeachersController(service);

            var createDto = new TeacherCreateDto
            {
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.com" // ✅ Valid email is essential
            };

            var result = await controller.CreateTeacher(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<TeacherDto>(createdResult.Value);

            Assert.Equal("Alice", returnValue.FirstName);
            Assert.Equal("Johnson", returnValue.LastName);
            Assert.Equal("alice.johnson@example.com", returnValue.Email);
        }

        [Fact]
        public async Task AddTeacher_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            var context = TestDbContextFactory.Create();
            var service = new TeacherService(context);
            var controller = new TeachersController(service);

            // Simulate invalid model state (e.g. missing email)
            controller.ModelState.AddModelError("Email", "Email is required");

            var createDto = new TeacherCreateDto
            {
                FirstName = "Ana",
                LastName = "Smith"
                // Email is intentionally missing
            };

            var result = await controller.CreateTeacher(createDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetTeacher_ShouldReturnTeacher_WhenTeacherExists()
        {
            var context = TestDbContextFactory.Create();
            var service = new TeacherService(context);
            var controller = new TeachersController(service);

            var teacher = new Teacher
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"  

            };
            context.Teachers.Add(teacher);
            context.SaveChanges();

            var result = await controller.GetTeacher(teacher.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var teacherDto = Assert.IsType<TeacherDto>(okResult.Value);
            Assert.Equal(teacher.FirstName, teacherDto.FirstName);
        }

        [Fact]
        public async Task GetTeacher_ShouldReturnNotFound_WhenTeacherDoesNotExist()
        {
            var context = TestDbContextFactory.Create();
            var service = new TeacherService(context);
            var controller = new TeachersController(service);

            var result = await controller.GetTeacher(9999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateTeacher_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            var context = TestDbContextFactory.Create();
            var service = new TeacherService(context);
            var controller = new TeachersController(service);

            var teacher = new Teacher
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            context.Teachers.Add(teacher);
            await context.SaveChangesAsync();

            var updateDto = new TeacherUpdateDto
            {
                Id = teacher.Id,  // Make sure this matches teacher.Id
                FirstName = "John",
                LastName = "Smith",  // updated last name
                Email = teacher.Email // if required, include Email
            };

            var result = await controller.UpdateTeacher(teacher.Id, updateDto);

            Assert.IsType<NoContentResult>(result);

            var updatedTeacher = await context.Teachers.FindAsync(teacher.Id);
            Assert.NotNull(updatedTeacher);
            Assert.Equal("Smith", updatedTeacher.LastName);
        }


        [Fact]
        public async Task UpdateTeacher_ShouldReturnBadRequest_WhenIdMismatch()
        {
            var context = TestDbContextFactory.Create();
            var service = new TeacherService(context);
            var controller = new TeachersController(service);

            var updateDto = new TeacherUpdateDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe"
            };

            var result = await controller.UpdateTeacher(2, updateDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Teacher ID mismatch", badRequest.Value);
        }

        [Fact]
        public async Task DeleteTeacher_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            var context = TestDbContextFactory.Create();
            var service = new TeacherService(context);
            var controller = new TeachersController(service);

            var teacher = new Teacher
            {
                FirstName = "Laura",
                LastName = "Wilson",
                Email= "Laura@gmail.com"
            };
            context.Teachers.Add(teacher);
            await context.SaveChangesAsync();

            var result = await controller.DeleteTeacher(teacher.Id);

            Assert.IsType<NoContentResult>(result);

            var deletedTeacher = await context.Teachers.FindAsync(teacher.Id);
            Assert.Null(deletedTeacher);
        }

        [Fact]
        public async Task DeleteTeacher_ShouldReturnNotFound_WhenTeacherDoesNotExist()
        {
            var context = TestDbContextFactory.Create();
            var service = new TeacherService(context);
            var controller = new TeachersController(service);

            var result = await controller.DeleteTeacher(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
