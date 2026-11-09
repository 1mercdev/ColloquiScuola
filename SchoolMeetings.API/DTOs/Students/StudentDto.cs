namespace SchoolMeetings.API.DTOs.Students
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}