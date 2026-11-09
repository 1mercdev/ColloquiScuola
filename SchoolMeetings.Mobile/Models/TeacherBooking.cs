namespace SchoolMeetings.Mobile.Models
{
    public class TeacherBooking
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

        public string MeetingTimeFormatted => MeetingDateTime.ToString("HH:mm");
        public string MeetingDateFormatted => MeetingDateTime.ToString("dddd dd MMMM yyyy");
        public string DurationFormatted => $"{DurationMinutes} minuti";
        public string StudentInfo => $"{StudentName} ({StudentClass})";
    }
}