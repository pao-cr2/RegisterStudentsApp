using RegisterStudents.API.Models;
using RegisterStudents.API.Services.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RegisterStudents.API.Dto;

namespace RegisterStudents.API.Services
{
    public interface IEnrollmentService
    {
        // Opcional: Exponer IQueryable solo si realmente se usa en consultas
        IQueryable<Enrollment> Enrollments { get; }
        IQueryable<Student> Students { get; }
        IQueryable<Subject> Subjects { get; }

        Task<ServiceResult<bool>> ValidateEnrollmentAsync(int studentId, int subjectId);

        Task<ServiceResult<Enrollment>> EnrollStudentInSubjectAsync(int studentId, int subjectId);

        Task<ServiceResult<Enrollment>> GetEnrollmentByIdAsync(int id);

        Task<ServiceResult<List<Enrollment>>> GetAllEnrollmentsAsync();

        Task<ServiceResult<Enrollment>> CreateEnrollmentAsync(EnrollmentCreateDto dto);

        Task<ServiceResult<bool>> UpdateEnrollmentAsync(Enrollment enrollment);

        Task<ServiceResult<bool>> DeleteEnrollmentAsync(int id);

        Task SaveChangesAsync();
    }
}
