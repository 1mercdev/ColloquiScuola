namespace SchoolMeetings.API.DTOs.Admin
{
    public class CleanupResultDto
    {
        public int BookingsDeleted { get; set; }
        public int TimeSlotsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}