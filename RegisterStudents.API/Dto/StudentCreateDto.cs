using System.ComponentModel.DataAnnotations;

namespace RegisterStudents.API.Dto
{
    public class StudentCreateDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int Credits { get; set; }

        [Required]
        public int TeacherId { get; set; }
    }
}
