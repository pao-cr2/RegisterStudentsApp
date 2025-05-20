using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RegisterStudents.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet("me")]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new { userId, email, role });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnrollmentReadDto>>> GetEnrollments()
        {
            var result = await _enrollmentService.GetAllEnrollmentsAsync();

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var enrollmentDtos = result.Data.Select(e => new EnrollmentReadDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentFirstName = e.Student?.FirstName ?? "Unknown",
                StudentLastName = e.Student?.LastName ?? "",
                StudentEmail = e.Student?.Email ?? "",
                SubjectId = e.SubjectId,
                SubjectName = e.Subject?.Name ?? "Unknown Subject",
                SubjectCredits = e.Subject?.Credits ?? 0,
                TeacherName = GetFullName(
                    e.Subject?.Teacher?.FirstName,
                    e.Subject?.Teacher?.LastName,
                    "Unknown Teacher"
                )
            }).ToList();

            return Ok(enrollmentDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EnrollmentReadDto>> GetEnrollment(int id)
        {
            var result = await _enrollmentService.GetEnrollmentByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var enrollment = result.Data;
            if (enrollment == null)
                return NotFound();

            var enrollmentDto = new EnrollmentReadDto
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                StudentFirstName = enrollment.Student?.FirstName ?? "Unknown",
                StudentLastName = enrollment.Student?.LastName ?? "",
                StudentEmail = enrollment.Student?.Email ?? "",
                SubjectId = enrollment.SubjectId,
                SubjectName = enrollment.Subject?.Name ?? "Unknown Subject",
                SubjectCredits = enrollment.Subject?.Credits ?? 0,
                TeacherName = GetFullName(
                    enrollment.Subject?.Teacher?.FirstName,
                    enrollment.Subject?.Teacher?.LastName,
                    "Unknown Teacher"
                )
            };

            return Ok(enrollmentDto);
        }

        [HttpPost]
        public async Task<IActionResult> PostEnrollment([FromBody] EnrollmentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _enrollmentService.CreateEnrollmentAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            var createdEnrollment = result.Data;
            if (createdEnrollment == null)
                return BadRequest("No se pudo crear la inscripción.");

            var enrollmentDto = new EnrollmentReadDto
            {
                Id = createdEnrollment.Id,
                StudentId = createdEnrollment.StudentId,
                StudentFirstName = createdEnrollment.Student?.FirstName ?? "N/A",
                StudentLastName = createdEnrollment.Student?.LastName ?? "N/A",
                StudentEmail = createdEnrollment.Student?.Email ?? "N/A",
                SubjectId = createdEnrollment.SubjectId,
                SubjectName = createdEnrollment.Subject?.Name ?? "Unknown Subject",
                SubjectCredits = createdEnrollment.Subject?.Credits ?? 0,
                TeacherName = GetFullName(
                    createdEnrollment.Subject?.Teacher?.FirstName,
                    createdEnrollment.Subject?.Teacher?.LastName,
                    "Unknown Teacher"
                )
            };

            return CreatedAtAction(nameof(GetEnrollment), new { id = enrollmentDto.Id }, enrollmentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnrollment(int id, [FromBody] EnrollmentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _enrollmentService.GetEnrollmentByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var existingEnrollment = result.Data;
            if (existingEnrollment == null)
                return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || userIdClaim != existingEnrollment.StudentId.ToString())
                return Forbid("No tienes permiso para modificar esta inscripción.");

            existingEnrollment.StudentId = dto.StudentId;
            existingEnrollment.SubjectId = dto.SubjectId;

            var updatedResult = await _enrollmentService.UpdateEnrollmentAsync(existingEnrollment);
            if (!updatedResult.IsSuccess || !updatedResult.Data)
                return BadRequest("No se pudo actualizar la inscripción.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var result = await _enrollmentService.GetEnrollmentByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            var existingEnrollment = result.Data;
            if (existingEnrollment == null)
                return NotFound();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || userIdClaim != existingEnrollment.StudentId.ToString())
                return Forbid("No tienes permiso para eliminar esta inscripción.");

            var deletedResult = await _enrollmentService.DeleteEnrollmentAsync(id);

            if (!deletedResult.IsSuccess || !deletedResult.Data)
                return BadRequest("No se pudo eliminar la inscripción.");

            return NoContent();
        }


        private string GetFullName(string? firstName, string? lastName, string defaultValue = "Unknown")
        {
            var fullName = $"{firstName ?? ""} {lastName ?? ""}".Trim();
            return string.IsNullOrWhiteSpace(fullName) ? defaultValue : fullName;
        }
    }
}
