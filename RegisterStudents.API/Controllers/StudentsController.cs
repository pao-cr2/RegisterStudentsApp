using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegisterStudents.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        [HttpGet]
        public async Task<ActionResult<List<StudentReadDto>>> GetStudents()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentReadDto>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpPost]
        public async Task<ActionResult<StudentReadDto>> CreateStudent(StudentCreateDto studentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _studentService.CreateStudentAsync(studentDto);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            // Accede al objeto creado a través de result.Data
            var createdStudent = result.Data;

            return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.Id }, createdStudent);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudent(int id, StudentUpdateDto studentDto)
        {
            if (id != studentDto.Id) return BadRequest();

            var exists = await _studentService.StudentExistsAsync(id);
            if (!exists) return NotFound();

            await _studentService.UpdateStudentAsync(studentDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var exists = await _studentService.StudentExistsAsync(id);
            if (!exists) return NotFound();

            await _studentService.DeleteStudentAsync(id);
            return NoContent();
        }
    }
}
