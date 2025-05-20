using RegisterStudents.API.Dto;
using RegisterStudents.API.Services.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegisterStudents.API.Services
{
    public interface IStudentService
    {
        Task<ServiceResult<List<StudentReadDto>>> GetAllStudentsAsync();
        Task<ServiceResult<StudentReadDto>> GetStudentByIdAsync(int id);
        Task<ServiceResult<StudentReadDto>> CreateStudentAsync(StudentCreateDto studentDto);
        Task<ServiceResult<bool>> UpdateStudentAsync(StudentUpdateDto studentDto);
        Task<ServiceResult<bool>> DeleteStudentAsync(int id);
        Task<bool> StudentExistsAsync(int id);
    }
}
