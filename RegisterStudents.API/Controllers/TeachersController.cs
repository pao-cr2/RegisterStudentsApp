using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegisterStudents.API.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeacherDto>>> GetTeachers()
        {
            var teachersResult = await _teacherService.GetAllTeachersAsync();

            if (!teachersResult.IsSuccess || teachersResult.Data == null)
                return NotFound();

            var teacherDtos = teachersResult.Data.Select(t => new TeacherDto
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email
            }).ToList();

            return Ok(teacherDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TeacherDto>> GetTeacher(int id)
        {
            var teacherResult = await _teacherService.GetTeacherByIdAsync(id);

            if (!teacherResult.IsSuccess || teacherResult.Data == null)
                return NotFound();

            var teacherDto = new TeacherDto
            {
                Id = teacherResult.Data.Id,
                FirstName = teacherResult.Data.FirstName,
                LastName = teacherResult.Data.LastName,
                Email = teacherResult.Data.Email
            };

            return Ok(teacherDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTeacher([FromBody] TeacherCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // <- This is essential

            var teacher = new Teacher
            {
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email
            };

            await _teacherService.CreateTeacherAsync(teacher);

            var teacherDto = new TeacherDto
            {
                Id = teacher.Id,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email
            };

            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.Id }, teacherDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTeacher(int id, TeacherUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("Teacher ID mismatch");

            var exists = await _teacherService.TeacherExistsAsync(id);
            if (!exists) return NotFound();

            var teacher = new Teacher
            {
                Id = id,
                FirstName = updateDto.FirstName,
                LastName = updateDto.LastName,
                Email = updateDto.Email
            };

            await _teacherService.UpdateTeacherAsync(teacher);
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTeacher(int id)
        {
            var exists = await _teacherService.TeacherExistsAsync(id);
            if (!exists) return NotFound();

            await _teacherService.DeleteTeacherAsync(id);
            return NoContent();
        }
    }

}
