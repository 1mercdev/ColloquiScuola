namespace SchoolMeetings.Mobile.Models
{
    public class UpdateTeacherRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Classroom { get; set; }
        public string? Notes { get; set; }
        public int MaxDailyBookings { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
    }
}