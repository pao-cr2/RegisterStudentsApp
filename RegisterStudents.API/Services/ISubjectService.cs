using RegisterStudents.API.Models;
using RegisterStudents.API.Services.Results;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RegisterStudents.API.Services
{
    public interface ISubjectService
    {
        IQueryable<Subject> Subjects { get; }

        Task<ServiceResult<Subject>> GetSubjectByIdAsync(int id);
        Task<ServiceResult<List<Subject>>> GetAllSubjectsAsync();
        Task<ServiceResult<Subject>> CreateSubjectAsync(Subject subject);   // <-- Aquí el cambio
        Task<ServiceResult<bool>> UpdateSubjectAsync(Subject subject);
        Task<ServiceResult<bool>> DeleteSubjectAsync(int id);
        Task<bool> SubjectExistsAsync(int id);
    }
}
