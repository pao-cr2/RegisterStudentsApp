using System.ComponentModel.DataAnnotations;

namespace RegisterStudents.API.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [StringLength(100)]
        public required string Name { get; set; }

        public required int Credits { get; set; }

        public int? TeacherId { get; set; }

        public Teacher? Teacher { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
