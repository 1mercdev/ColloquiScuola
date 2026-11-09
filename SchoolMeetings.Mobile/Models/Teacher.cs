namespace SchoolMeetings.Mobile.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string? Classroom { get; set; }
        public bool IsAvailable { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}