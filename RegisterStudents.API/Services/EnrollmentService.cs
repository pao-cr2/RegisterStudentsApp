using Microsoft.EntityFrameworkCore;
using RegisterStudents.API.Data;
using RegisterStudents.API.Dto;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegisterStudents.API.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly AppDbContext _context;

        public EnrollmentService(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Enrollment> Enrollments => _context.Enrollments;
        public IQueryable<Student> Students => _context.Students;
        public IQueryable<Subject> Subjects => _context.Subjects;

        public async Task<ServiceResult<bool>> ValidateEnrollmentAsync(int studentId, int subjectId)
        {
            var studentEnrollments = await _context.Enrollments
                .Include(e => e.Subject)
                    .ThenInclude(s => s.Teacher)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();

            if (studentEnrollments.Count >= 3)
                return ServiceResult<bool>.Failure(new[] { "El estudiante no puede inscribirse en más de 3 materias." });

            var subjectToEnroll = await _context.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subjectToEnroll == null)
                return ServiceResult<bool>.Failure(new[] { "La materia no existe." });

            var teacherId = subjectToEnroll.TeacherId;
            if (studentEnrollments.Any(e => e.Subject?.TeacherId == teacherId))
                return ServiceResult<bool>.Failure(new[] { "No se puede tener más de una materia con el mismo profesor." });

            return ServiceResult<bool>.Success(true);
        }
        public async Task<ServiceResult<Enrollment>> EnrollStudentInSubjectAsync(int studentId, int subjectId)
        {
            var validationResult = await ValidateEnrollmentAsync(studentId, subjectId);
            if (!validationResult.IsSuccess)
                return ServiceResult<Enrollment>.Failure(validationResult.Errors);

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                SubjectId = subjectId
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var createdEnrollment = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Subject)
                    .ThenInclude(s => s.Teacher)
                .FirstOrDefaultAsync(e => e.Id == enrollment.Id);

            return ServiceResult<Enrollment>.Success(createdEnrollment);
        }
        public async Task<ServiceResult<Enrollment>> GetEnrollmentByIdAsync(int id)
        {
            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Subject)
                    .ThenInclude(s => s.Teacher)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
                return ServiceResult<Enrollment>.Failure("La inscripción no existe.");

            return ServiceResult<Enrollment>.Success(enrollment);
        }
        public async Task<ServiceResult<List<Enrollment>>> GetAllEnrollmentsAsync()
        {
            var enrollments = await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Subject)
                    .ThenInclude(s => s.Teacher)
                .ToListAsync();

            return ServiceResult<List<Enrollment>>.Success(enrollments);
        }

        public async Task<ServiceResult<bool>> UpdateEnrollmentAsync(Enrollment enrollment)
        {
            var existingEnrollment = await _context.Enrollments.FindAsync(enrollment.Id);
            if (existingEnrollment == null)
                return ServiceResult<bool>.Failure(new[] { "La inscripción no existe." });

            // Actualizamos las propiedades que deseas modificar
            existingEnrollment.StudentId = enrollment.StudentId;
            existingEnrollment.SubjectId = enrollment.SubjectId;
            // ... otras propiedades si es necesario

            try
            {
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Log o manejo de error
                return ServiceResult<bool>.Failure(new[] { ex.Message });
            }
        }

        public async Task<ServiceResult<bool>> DeleteEnrollmentAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return ServiceResult<bool>.Failure(new[] { "La inscripción no existe." });

            try
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure(new[] { ex.Message });
            }
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceResult<Enrollment>> CreateEnrollmentAsync(EnrollmentCreateDto dto)
        {
            try
            {
                var enrollment = new Enrollment
                {
                    StudentId = dto.StudentId,
                    SubjectId = dto.SubjectId,
                    // otros campos si tienes
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();

                // Cargar las relaciones si las necesitas (opcional)
                await _context.Entry(enrollment).Reference(e => e.Student).LoadAsync();
                await _context.Entry(enrollment).Reference(e => e.Subject).LoadAsync();
                await _context.Entry(enrollment.Subject).Reference(s => s.Teacher).LoadAsync();

                return ServiceResult<Enrollment>.Success(enrollment);
            }
            catch (Exception ex)
            {
                // Aquí puedes loggear el error si quieres
                return ServiceResult<Enrollment>.Failure(new[] { ex.Message });
            }
        }

    }
}
