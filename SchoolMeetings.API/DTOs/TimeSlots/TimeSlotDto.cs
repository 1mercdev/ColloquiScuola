namespace SchoolMeetings.API.DTOs.TimeSlots
{
    public class TimeSlotDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Classroom { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}
}