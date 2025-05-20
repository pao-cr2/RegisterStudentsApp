using System.ComponentModel.DataAnnotations;

namespace RegisterStudents.API.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public Student? Student { get; set; }

        public Subject? Subject { get; set; }
    }

}
