namespace SchoolMeetings.Mobile.Models
{
    public class CleanupResult
    {
        public int BookingsDeleted { get; set; }
        public int TimeSlotsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}