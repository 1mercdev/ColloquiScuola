namespace SchoolMeetings.API.DTOs.Bookings
{
    public class TeacherBookingDto
    {
        public int Id { get; set; }
        public DateTime MeetingDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentClass { get; set; } = string.Empty;
        public string ParentName { get; set; } = string.Empty;
        public string ParentEmail { get; set; } = string.Empty;
        public string? ParentPhone { get; set; }
        public string? ParentNotes { get; set; }
        public string? TeacherNotes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}