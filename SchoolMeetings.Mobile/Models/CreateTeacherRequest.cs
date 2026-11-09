namespace SchoolMeetings.Mobile.Models
{
    public class CreateTeacherRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Classroom { get; set; }
        public string? Notes { get; set; }
        public int MaxDailyBookings { get; set; } = 0;
    }
}