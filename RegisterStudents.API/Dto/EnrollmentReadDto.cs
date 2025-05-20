namespace RegisterStudents.API.Dto
{
    public class EnrollmentReadDto
    {
        public int Id { get; set; }

        // Student details
        public int StudentId { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentEmail { get; set; }

        // Subject details
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int SubjectCredits { get; set; }

        // Teacher details
        public string TeacherName { get; set; }
    }
}
