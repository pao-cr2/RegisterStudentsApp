namespace RegisterStudents.API.Dto
{
    public class SubjectUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }   // <-- add this to match Subject class

    }
}
