using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegisterStudents.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SubjectDto>>> GetSubjects()
        {
            var result = await _subjectService.GetAllSubjectsAsync();

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            var subjectDtos = result.Data.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            return Ok(subjectDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SubjectReadDto>> GetSubject(int id)
        {
            var subjectResult = await _subjectService.GetSubjectByIdAsync(id);

            if (!subjectResult.IsSuccess || subjectResult.Data == null)
                return NotFound();

            var subjectReadDto = new SubjectReadDto
            {
                Id = subjectResult.Data.Id,
                Name = subjectResult.Data.Name,
                Credits = subjectResult.Data.Credits
            };

            return Ok(subjectReadDto);
        }


        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subject = new Subject
            {
                Name = dto.Name,
                Credits = dto.Credits
            };

            await _subjectService.CreateSubjectAsync(subject);

            var subjectReadDto = new SubjectReadDto
            {
                Id = subject.Id,  // populated after SaveChangesAsync
                Name = subject.Name,
                Credits = subject.Credits
            };

            return CreatedAtAction(nameof(GetSubject), new { id = subjectReadDto.Id }, subjectReadDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSubject(int id, SubjectUpdateDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest("Subject ID mismatch");

            var exists = await _subjectService.SubjectExistsAsync(id);
            if (!exists) return NotFound();

            var subject = new Subject
            {
                Id = id,
                Name = updateDto.Name,
                Credits = updateDto.Credits
            };

            await _subjectService.UpdateSubjectAsync(subject);
            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSubject(int id)
        {
            var exists = await _subjectService.SubjectExistsAsync(id);
            if (!exists) return NotFound();

            await _subjectService.DeleteSubjectAsync(id);
            return NoContent();
        }
    }

}
