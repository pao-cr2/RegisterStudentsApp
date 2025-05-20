using System.ComponentModel.DataAnnotations;

namespace RegisterStudents.API.Models
{
    public class Teacher
    {
        public int Id { get; set; }

        [StringLength(50)]
        public required string FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(50)]
        public required string LastName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public ICollection<Subject>? Subjects { get; set; }
    }
}
