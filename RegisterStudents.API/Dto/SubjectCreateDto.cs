namespace RegisterStudents.API.Dto
{
    public class SubjectCreateDto
    {
        public string Name { get; set; }
        public int Credits { get; set; }   // <-- add this to match Subject class

    }
}
