using Microsoft.AspNetCore.Mvc;
using RegisterStudents.API.Controllers;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services;
using RegisterStudents.Tests.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace RegisterStudents.Tests.Controllers
{
    public class SubjectsControllerTests
    {
        [Fact]
        public async Task AddSubject_ShouldReturnCreatedAtAction_WhenValidSubject()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubjectService(context);             // Use service, NOT context
            var controller = new SubjectsController(service);

            var validSubjectDto = new SubjectCreateDto   // <-- Fixed this line
            {
                Name = "Mathematics",
                Credits = 3
            };

            var result = await controller.CreateSubject(validSubjectDto);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result); 
            var createdSubject = Assert.IsType<SubjectReadDto>(createdAtActionResult.Value);
            Assert.Equal("Mathematics", createdSubject.Name);
        }


        [Fact]
        public async Task AddSubject_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubjectService(context);
            var controller = new SubjectsController(service);

            controller.ModelState.AddModelError("Name", "The Name field is required.");

            var invalidSubjectDto = new SubjectCreateDto
            {
                Name = "", // invalid empty name
                Credits = 0
            };

            var result = await controller.CreateSubject(invalidSubjectDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetSubject_ShouldReturnSubject_WhenSubjectExists()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubjectService(context);
            var controller = new SubjectsController(service);

            var subject = new Subject { Name = "Math", Credits = 3 };
            context.Subjects.Add(subject);
            context.SaveChanges();

            var result = await controller.GetSubject(subject.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var subjectDto = Assert.IsType<SubjectReadDto>(okResult.Value);
            Assert.Equal(subject.Name, subjectDto.Name);
        }

        [Fact]
        public async Task GetSubject_ShouldReturnNotFound_WhenSubjectDoesNotExist()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubjectService(context);
            var controller = new SubjectsController(service);

            var result = await controller.GetSubject(9999);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateSubject_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubjectService(context);
            var controller = new SubjectsController(service);

            var subject = new Subject
            {
                Name = "Physics",
                Credits = 3
            };
            context.Subjects.Add(subject);
            await context.SaveChangesAsync();

            var updateDto = new SubjectUpdateDto
            {
                Id = subject.Id,            // ✅ This fixes the issue
                Name = subject.Name,
                Credits = 4
            };

            var result = await controller.UpdateSubject(subject.Id, updateDto);

            Assert.IsType<NoContentResult>(result);

            var updatedSubject = await context.Subjects.FindAsync(subject.Id);
            Assert.NotNull(updatedSubject);
            Assert.Equal(4, updatedSubject.Credits);
        }


        [Fact]
        public async Task UpdateSubject_ShouldReturnBadRequest_WhenIdMismatch()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubjectService(context);
            var controller = new SubjectsController(service);

            var updateDto = new SubjectUpdateDto
            {
                Name = "Math",
                Credits = 3
            };

            var result = await controller.UpdateSubject(2, updateDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Subject ID mismatch", badRequest.Value);
        }

        [Fact]
        public async Task DeleteSubject_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubjectService(context);
            var controller = new SubjectsController(service);

            var subject = new Subject
            {
                Name = "History",
                Credits = 2
            };
            context.Subjects.Add(subject);
            await context.SaveChangesAsync();

            var result = await controller.DeleteSubject(subject.Id);

            Assert.IsType<NoContentResult>(result);

            var deletedSubject = await context.Subjects.FindAsync(subject.Id);
            Assert.Null(deletedSubject);
        }

        [Fact]
        public async Task DeleteSubject_ShouldReturnNotFound_WhenSubjectDoesNotExist()
        {
            var context = TestDbContextFactory.Create();
            var service = new SubjectService(context);
            var controller = new SubjectsController(service);

            var result = await controller.DeleteSubject(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
