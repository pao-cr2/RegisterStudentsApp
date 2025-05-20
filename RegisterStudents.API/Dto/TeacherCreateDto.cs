using System.ComponentModel.DataAnnotations;

namespace RegisterStudents.API.Dto
{
    public class TeacherCreateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
