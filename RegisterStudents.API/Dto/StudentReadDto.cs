namespace RegisterStudents.API.Dto
{
    public class StudentReadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Credits { get; set; }
        public int TeacherId { get; set; }
    }
}
