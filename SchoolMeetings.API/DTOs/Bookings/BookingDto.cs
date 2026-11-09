namespace SchoolMeetings.API.DTOs.Bookings
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int TimeSlotId { get; set; }
        public DateTime MeetingDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentClass { get; set; } = string.Empty;
        public string? ParentNotes { get; set; }
        public string? TeacherNotes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}