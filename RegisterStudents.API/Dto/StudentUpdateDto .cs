using System.ComponentModel.DataAnnotations;

namespace RegisterStudents.API.Dto
{
    public class StudentUpdateDto : StudentCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
