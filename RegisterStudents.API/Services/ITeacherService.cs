using RegisterStudents.API.Models;
using RegisterStudents.API.Services.Results;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RegisterStudents.API.Services
{
    public interface ITeacherService
    {
        IQueryable<Teacher> Teachers { get; }

        Task<ServiceResult<Teacher>> GetTeacherByIdAsync(int id);
        Task<ServiceResult<List<Teacher>>> GetAllTeachersAsync();
        Task<ServiceResult<Teacher>> CreateTeacherAsync(Teacher teacher);
        Task<ServiceResult<bool>> UpdateTeacherAsync(Teacher teacher);
        Task<ServiceResult<bool>> DeleteTeacherAsync(int id);
        Task<bool> TeacherExistsAsync(int id);
    }
}
