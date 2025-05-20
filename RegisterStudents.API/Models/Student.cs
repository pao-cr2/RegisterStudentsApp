using System.ComponentModel.DataAnnotations;

namespace RegisterStudents.API.Models
{
    public class Student
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
        public required int Credits { get; set; }

        public required int TeacherId { get; set; }
        
        public Teacher? Teacher { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
